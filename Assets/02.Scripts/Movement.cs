using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    CharacterController controller;
    new Transform transform;    // 
    Animator animator;
    new Camera camera;

    Plane plane; // 가상의 바닥에 레이캐스팅 하기 위한 변수
    Ray ray;
    Vector3 hitPoint;

    public float moveSpeed = 10f;
    PhotonView pv = null;
    CinemachineVirtualCamera virtualCamera;
    Vector3 CurPos;
    Quaternion CurRot;
    public float damping = 10f;
    Transform tr;

    void Start()
    {
        //pv = GetComponent<PhotonView>();
        pv = PhotonView.Get(this);
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        tr = this.transform;

        if(pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

        // 가상의 바닥을 주인공 위치의 위에 생성
        plane = new Plane(transform.up, transform.position);
    }

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");
    
    void Update()
    {
        if(pv.IsMine)
        {
            Move();
            Turn();
        }
        else
        {
            tr.position = Vector3.Lerp(tr.position, CurPos, Time.deltaTime * damping);
            tr.rotation = Quaternion.Slerp(tr.rotation, CurRot, Time.deltaTime * damping);
        }
        
    }

    void Move()
    {
        Vector3 CameraForward = camera.transform.forward;   
        Vector3 CameraRight = camera.transform.right;
        CameraForward.y = 0f;
        CameraRight.y = 0f;
        // 캐릭터 이동 방향 벡터 계산
        Vector3 moveDir = (CameraForward * v) + (CameraRight * h);
        moveDir.Set(moveDir.x, 0f, moveDir.z);
        // 카메라와 같이 이동
        controller.SimpleMove(moveDir * moveSpeed);
        // 캐릭터의 애니메이션 처리
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }
    void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter = 0f;
        // 가상의 바닥에 광선을 발사해 충돌된 지점의 거리를 enter 변수로 변환
        plane.Raycast(ray, out enter);
        // 가상의 바닥에 광선이 충돌한 좌표값 추출
        hitPoint = ray.GetPoint(enter);

        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;
        // 주인공 캐릭터의 회전값 지정
        transform.localRotation = Quaternion.LookRotation(lookDir);
    }

    // 송수신 해주는 메소드, 내 움직임은 송신, 다른 네트워크 유저의 움직임은 수신
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) // 송신
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else if(stream.IsReading) // 수신
        {
            CurPos = (Vector3)stream.ReceiveNext();
            CurRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

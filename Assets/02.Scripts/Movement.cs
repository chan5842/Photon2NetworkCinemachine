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

    Plane plane; // ������ �ٴڿ� ����ĳ���� �ϱ� ���� ����
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

        // ������ �ٴ��� ���ΰ� ��ġ�� ���� ����
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
        // ĳ���� �̵� ���� ���� ���
        Vector3 moveDir = (CameraForward * v) + (CameraRight * h);
        moveDir.Set(moveDir.x, 0f, moveDir.z);
        // ī�޶�� ���� �̵�
        controller.SimpleMove(moveDir * moveSpeed);
        // ĳ������ �ִϸ��̼� ó��
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }
    void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter = 0f;
        // ������ �ٴڿ� ������ �߻��� �浹�� ������ �Ÿ��� enter ������ ��ȯ
        plane.Raycast(ray, out enter);
        // ������ �ٴڿ� ������ �浹�� ��ǥ�� ����
        hitPoint = ray.GetPoint(enter);

        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;
        // ���ΰ� ĳ������ ȸ���� ����
        transform.localRotation = Quaternion.LookRotation(lookDir);
    }

    // �ۼ��� ���ִ� �޼ҵ�, �� �������� �۽�, �ٸ� ��Ʈ��ũ ������ �������� ����
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) // �۽�
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else if(stream.IsReading) // ����
        {
            CurPos = (Vector3)stream.ReceiveNext();
            CurRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

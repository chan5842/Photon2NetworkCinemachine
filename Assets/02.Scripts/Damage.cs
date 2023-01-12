using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    Renderer[] renderers;
    int initHp = 100;
    public int curHp = 0;
    Animator animator;
    CharacterController controller;

    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashRespawn = Animator.StringToHash("Respawn");
    readonly string bulletTag = "BULLET";

    void Awake()
    {
        curHp = initHp;
        renderers = GetComponentsInChildren<MeshRenderer>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (curHp > 0 && col.collider.CompareTag(bulletTag))
        {
            curHp -= 20;
            if (curHp <= 0)
            {
                StartCoroutine("PlayerDie");
            }       
        }
    }
    IEnumerator PlayerDie()
    {
        controller.enabled = false;
        animator.SetBool(hashRespawn, false);
        animator.SetTrigger(hashDie);

        yield return new WaitForSeconds(5f);
        animator.SetBool(hashRespawn, true);
        SetPlayerVisible(false);
        yield return new WaitForSeconds(1.5f);

        Transform[] points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>(); // 플레이어 출현 위치 정보를 배열에 저장
        int idx = Random.Range(1, points.Length);
        transform.position = points[idx].position;
        curHp = initHp;
        SetPlayerVisible(true);
        controller.enabled = true;
    }
    void SetPlayerVisible(bool visible)
    {
        foreach(var renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }
    void Update()
    {
        
    }
}

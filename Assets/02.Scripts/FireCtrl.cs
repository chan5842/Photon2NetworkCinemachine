using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FireCtrl : MonoBehaviour
{
    public Transform FirePos;
    public GameObject bulletPrefab;
    ParticleSystem muzzleflash;
    PhotonView pv;
    bool isMouseClick => Input.GetMouseButtonDown(0);


    void Start()
    {
        pv = PhotonView.Get(this);
        muzzleflash = FirePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(pv.IsMine && isMouseClick)
        {
            FireBullet();
            pv.RPC("FireBullet", RpcTarget.Others, null);
        }
    }

    [PunRPC]
    void FireBullet()
    {
        if (!muzzleflash.isPlaying) muzzleflash.Play(true);
        GameObject bullet = Instantiate(bulletPrefab, FirePos.position, FirePos.rotation);
    }
}

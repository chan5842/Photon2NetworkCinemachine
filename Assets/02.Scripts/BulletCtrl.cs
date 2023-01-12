using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public GameObject Effect;

    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000f);
        Destroy(this.gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var contact = collision.GetContact(0);
        var obj = Instantiate(Effect, contact.point, Quaternion.LookRotation(-contact.normal));
        Destroy(obj, 2.0f);
        Destroy(this.gameObject);
    }
}

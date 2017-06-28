using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public GameObject expEffect;
    private Transform tr;
    public AudioClip expSfx;
    public int damage = 20;
    public float speed = 1000.0f;
    public Vector3 firePos;

    private void Start()
    {
        tr = GetComponent<Transform>();
        GetComponent<Rigidbody>().AddForce((transform.forward + Vector3.up * 0.2f) * speed); // 월드좌표기준
        //GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed); // 로컬좌표기준
        firePos = transform.position;
        StartCoroutine(this.BulletTimer());
    }

    private IEnumerator BulletTimer()
    {
        yield return new WaitForSeconds(3.0f);
        ExpBullet();
    }

    private void OnCollisionEnter(Collision coll)
    {
        ExpBullet();
    }

    private void ExpBullet()
    {
        GameMgr.instance.PlaySfx(tr.transform.position, expSfx);
        GameObject explosion = (GameObject)Instantiate(expEffect, tr.position, Quaternion.identity);
        Destroy(explosion, 5.0f);
        Collider[] colls = Physics.OverlapSphere(tr.position, 5.0f);
        foreach (Collider coll in colls)
        {
            coll.SendMessage("OnExpDamage", SendMessageOptions.DontRequireReceiver);
            Rigidbody rBody = coll.GetComponent<Rigidbody>();
            if (rBody != null)
            {
                rBody.mass = 1.0f;
                rBody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
            }
        }
        Destroy(this.gameObject);
    }
}
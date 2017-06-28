using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;
    public GameObject sparkEffect;
    public GameObject fireEffect;
    public Texture[] textures;
    public AudioClip expSfx;
    public Transform firePivot;
    private Transform tr;
    private int hitCount = 0;
    //private AudioSource source = null;

    private void Start()
    {
        tr = GetComponent<Transform>();
        //source = GetComponent<AudioSource>();
        int idx = Random.Range(0, textures.Length - 1);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];
    }

    //private void OnCollisionEnter(Collision coll)
    //{
    //    if (coll.collider.tag == "BULLET")
    //    {
    //        Destroy(coll.gameObject);
    //        GameObject spark = (GameObject)Instantiate(sparkEffect,
    //            coll.transform.position, Quaternion.identity);
    //        Destroy(spark, 0.2f);
    //        if (++hitCount >= 3)
    //        {
    //            ExpBarrel();
    //            //source.PlayOneShot(expSfx, 1.0f);
    //            GameMgr.instance.PlaySfx(tr.position, expSfx);
    //        }
    //    }
    //}

    private void ExpBarrel()
    {
        GameMgr.instance.PlaySfx(tr.position, expSfx);
        GameObject exp = (GameObject)Instantiate(expEffect, tr.position, Quaternion.identity);
        GameObject fireExp = (GameObject)Instantiate(fireEffect, firePivot.position, Quaternion.identity);
        fireExp.transform.parent = tr;
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[4];
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f);
        foreach (var item in colls)
        {
            item.SendMessage("OnExpDamage", SendMessageOptions.DontRequireReceiver);
            Rigidbody rbody = item.GetComponent<Rigidbody>();
            if (rbody != null)
            {
                rbody.mass = 1.0f;
                rbody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
            }
        }
        Destroy(exp, 4.0f);
        Destroy(gameObject, 8.0f);
    }

    private void OnDamage(object[] _params)
    {
        GameObject spark = (GameObject)Instantiate(sparkEffect,
            (Vector3)_params[0], Quaternion.identity);
        Destroy(spark, 0.2f);
        Vector3 hitPos = (Vector3)_params[0]; // 맞은위치
        Vector3 firePos = (Vector3)_params[1]; // 발사원점
        Vector3 incomVector = hitPos - firePos; // 입사각 = 맞은 위치 - 발사원점
        incomVector = incomVector.normalized;  // 단위 벡터로 정규화 시킴.
        GetComponent<Rigidbody>().AddForceAtPosition(incomVector * 1000.0f, hitPos);
        if (++hitCount >= 3) ExpBarrel();
    }

    private void OnExpDamage()
    {
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[textures.Length - 1];
        GameObject fire = (GameObject)Instantiate(fireEffect, tr.position, Quaternion.identity);
        fire.transform.parent = firePivot;
        Destroy(fire, 5.0f);
        StartCoroutine(DelayExp());
    }

    private IEnumerator DelayExp()
    {
        yield return new WaitForSeconds(5.0f);
        ExpBarrel();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour
{
    public GameObject sparkEffect;
    private Vector3 firePos;

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "BULLET")
        {
            firePos = coll.gameObject.GetComponent<BulletCtrl>().firePos;
            Vector3 relativePos = transform.position - firePos;
            GameObject spark = (GameObject)Instantiate(sparkEffect,
                coll.transform.position, Quaternion.LookRotation(relativePos));
            Destroy(spark, 2.0f);
            Destroy(coll.gameObject);
        }
    }

    private void OnDamage(object[] _params)
    {
        firePos = (Vector3)_params[1];
        Vector3 relativePos = transform.position - firePos;
        GameObject spark = (GameObject)Instantiate(sparkEffect,
            (Vector3)_params[0], Quaternion.LookRotation(relativePos));
        Destroy(spark, 2.0f);
    }
}
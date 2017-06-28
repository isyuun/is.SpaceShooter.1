using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;
    public GameObject sparkEffect;
    public Transform firePos;
    public AudioClip fireSfx;
    public MeshRenderer muzzleFlash;
    public float nextFire = 0.1f;
    private float fireTime = 0.0f;
    private AudioSource source = null;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        muzzleFlash.enabled = false;
    }

    private void Update()
    {
        // Debug.DrawRay(firePos.position, firePos.forward * 20.0f, Color.green);

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            fireTime += Time.deltaTime;
            if (fireTime >= nextFire)
            {
                fireTime = 0.0f;
                Fire(0);
                if (Physics.Raycast(firePos.position, firePos.forward, out hit, 50.0f))
                {
                    //if (hit.collider.tag == "MONSTER" || hit.collider.tag == "BARREL")
                    //{
                    //    object[] _params = new object[2];
                    //    _params[0] = hit.point;  // 레이가 맞은 위치값(vector3)
                    //    if (hit.collider.tag == "MONSTER") _params[1] = 20;   // 데미지 값
                    //    else _params[1] = firePos.position;   // 발사 원점
                    //    hit.collider.gameObject.SendMessage("OnDamage", _params,
                    //        SendMessageOptions.DontRequireReceiver);
                    //}
                    object[] _params = new object[2];
                    _params[0] = hit.point;  // 레이가 맞은 위치값(vector3)
                    if (hit.collider.tag == "MONSTER" || hit.collider.tag == "MAX") _params[1] = 20;   // 데미지 값
                    else _params[1] = firePos.position;   // 발사 원점
                    hit.collider.gameObject.SendMessage("OnDamage", _params,
                        SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Fire(1);
        }
    }

    private void Fire(int ch)
    {
        if (ch == 1) CreatBullet();
        //source.PlayOneShot(fireSfx, 0.9f);
        GameMgr.instance.PlaySfx(firePos.position, fireSfx);
        GameObject spark = (GameObject)Instantiate(sparkEffect,
                firePos.position, Quaternion.identity);
        Destroy(spark, 0.5f);
        StartCoroutine(this.ShowMuzzleFlash());
    }

    private IEnumerator ShowMuzzleFlash()
    {
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));
        muzzleFlash.enabled = false;
    }

    private void CreatBullet()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
    }
}
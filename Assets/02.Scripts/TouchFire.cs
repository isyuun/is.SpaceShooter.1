using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchFire : MonoBehaviour
{
    public GameObject expEffect;
    public AudioClip expSfx;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        RaycastHit hit;
        Ray ray;

#if UNITY_EDITOR
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.tag == "EXP_BOX")
                {
                    ExpBox(hit);
                }
                Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "MAX")
                {
                    hit.collider.gameObject.SendMessage("OnHit",
                        SendMessageOptions.DontRequireReceiver);
                }
            }
        }
#endif

        // #if UNITY_IPHONE
#if UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.tag == "EXP_BOX")
                {
                    ExpBox(hit);
                }
            }
        }
#endif
    }

    private void ExpBox(RaycastHit hit)
    {
        GameObject exp = (GameObject)Instantiate(expEffect, hit.transform.position, Quaternion.identity);
        //GameObject soundObj = new GameObject("Sfx");
        //soundObj.transform.position = hit.point;
        AudioSource _audioSource = exp.AddComponent<AudioSource>();
        _audioSource.clip = expSfx;
        _audioSource.minDistance = 10.0f;
        _audioSource.maxDistance = 30.0f;
        _audioSource.volume = 1.0f;
        _audioSource.Play();
        //Destroy(soundObj, expSfx.length);
        //Destroy(exp, 3.0f);
        Destroy(exp, 5.0f);
        Destroy(hit.collider.gameObject);
    }
}
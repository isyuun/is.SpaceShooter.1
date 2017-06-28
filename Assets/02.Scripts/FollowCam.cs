using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    public float dist = 5.0f;
    public float height = 2.3f;
    public float dampTrace = 20.0f;
    private Transform tr;
    private RaycastHit hit;
    private float camDist;

    // Use this for initialization
    private void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //tr.position = Vector3.Lerp(tr.position,
        //    targetTr.position - (targetTr.forward * dist) + (Vector3.up * height),
        //    dampTrace * Time.deltaTime);
        Ray ray = new Ray(targetTr.position, -tr.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);
        Physics.Raycast(ray, out hit, Mathf.Infinity);

        camDist = Vector3.Distance(targetTr.position, hit.point);
        if (camDist <= dist) tr.position = Vector3.Lerp(tr.position, targetTr.position
            - (targetTr.forward * camDist) + (Vector3.up * height), Time.deltaTime * dampTrace);
        else tr.position = Vector3.Lerp(tr.position,
            targetTr.position - (targetTr.forward * dist) + (Vector3.up * height),
            Time.deltaTime * dampTrace);

        tr.LookAt(targetTr.position);
    }
}
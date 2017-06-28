using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private Transform tr;
    private LineRenderer line;
    private RaycastHit hit;
    public float nextFire = 0.1f;
    private float fireTime = 0.0f;

    // Use this for initialization
    private void Start()
    {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.enabled = false;
        line.SetWidth(0.1f, 0.02f);
    }

    // Update is called once per frame
    private void Update()
    {
        Ray ray = new Ray(tr.position, tr.forward);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (Input.GetMouseButton(0))
        {
            fireTime += Time.deltaTime;
            if (fireTime >= nextFire)
            {
                line.SetPosition(0, tr.InverseTransformPoint(ray.origin)); // 월드좌표를 로컬좌표로 변경
                if (Physics.Raycast(ray, out hit, 100.0f /*Mathf.Infinity*/))
                {
                    line.SetPosition(1, tr.InverseTransformPoint(hit.point));
                }
                else line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100.0f)));
                StartCoroutine(this.ShowLaserBeam());
            }
        }
    }

    private IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.05f));
        line.enabled = false;
    }
}
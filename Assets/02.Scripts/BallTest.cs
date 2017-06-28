using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTest : MonoBehaviour
{
    private Transform tr;
    private Rigidbody rBody;

    private void Start()
    {
        tr = GetComponent<Transform>();
        rBody = GetComponent<Rigidbody>();
        rBody.AddForce(tr.forward * 500);
        rBody.AddTorque(tr.right * 100);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagdollMove : MonoBehaviour
{
    public Rigidbody[] rBody;
    private Animation _animation;
    private Transform maxTr;

    // Use this for initialization
    private void Start()
    {
        _animation = GetComponent<Animation>();
        maxTr = this.gameObject.GetComponent<Transform>();
        rBody = GetComponentsInChildren<Rigidbody>();
        //StartCoroutine(this.WakeupRagdoll());
    }

    private IEnumerator WakeupRagdoll()
    {
        //yield return new WaitForSeconds(3.0f);
        GetComponent<TouchMove>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        _animation.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(3.0f);
        SetRagdoll(true);
    }

    private void SetRagdoll(bool isEnable)
    {
        GetComponent<TouchMove>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        _animation.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        foreach (Rigidbody _rbody in rBody)
        {
            _rbody.isKinematic = !isEnable;
        }
    }

    private void OnHit()
    {
        Debug.Log("Max Hit!!!");
        SetRagdoll(true);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    [ContextMenu("ContextTest")]
    private void ContextTest()
    {
        Debug.Log("Test!!!");
    }
}
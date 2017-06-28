using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Max_Anim
{
    public AnimationClip idle;
    public AnimationClip run;
    public AnimationClip walk;
    public AnimationClip flip;
    public AnimationClip punch;
    public AnimationClip kick;
    public AnimationClip death;
    public AnimationClip jump;
}

public class TouchMove : MonoBehaviour
{
    public Max_Anim anim;
    public Animation _animation;
    private Transform tr;
    private Vector3 target;
    private NavMeshAgent nvAgent;

    // Use this for initialization
    private void Start()
    {
        tr = GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        _animation = GetComponent<Animation>();
        _animation.clip = anim.idle;
        target = tr.position;
    }

    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red);
        RaycastHit hit;

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("FLOOR")))
            {
                target = hit.point;
                nvAgent.SetDestination(target);
            }
        }
        float dist = Vector3.Distance(tr.position, target);
        //Debug.Log(dist);
        if (dist > 3.0f)
        {
            nvAgent.speed = 4.4f;
            _animation.CrossFade(anim.run.name, 0.3f);
        }
        else if (dist > 0.2f)
        {
            nvAgent.speed = 2.5f;
            _animation.CrossFade(anim.walk.name, 0.3f);
        }
        else { _animation.CrossFade(anim.idle.name, 0.3f); }
    }

    private void OnHit()
    {
        nvAgent.enabled = false;
        _animation.enabled = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MaxAnim
{
    public AnimationClip idle;
    public AnimationClip run;
    public AnimationClip walk;
    public AnimationClip punch;
    public AnimationClip kick;
    public AnimationClip flip;
    public AnimationClip die;
}

public class RagdollCtrl : MonoBehaviour
{
    public Rigidbody[] rBody;
    public MaxAnim _maxAnim;
    private Animation _animation;
    private Transform maxTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    public Transform firePivot;
    public float traceDist = 50.0f;
    public float attackDist = 2.0f;
    private bool isDie = false;
    public GameObject bloodEffect;
    public GameObject bloodDecal;
    public GameObject fireEffect;
    private int hp = 100;
    private bool expChk = false;
    private GameUI _gameUI;
    public Material fireMax;
    public Material normalMax;
    private SkinnedMeshRenderer[] skinMesh;

    // Use this for initialization
    private void Awake()
    {
        _animation = GetComponent<Animation>();
        rBody = GetComponentsInChildren<Rigidbody>();
        maxTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        _gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
        skinMesh = GetComponentsInChildren<SkinnedMeshRenderer>();
        SetRagdoll(false);
        //StartCoroutine(this.WakeupRagdoll());
        _animation.clip = _maxAnim.idle;
        _animation.Play();
    }

    private void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
        StartCoroutine(this.CheckMaxState());
    }

    private void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    private void OnPlayerDie()
    {
        StopAllCoroutines();
        nvAgent.Stop();
        _animation.clip = _maxAnim.kick;
        _animation.Play();
    }

    private IEnumerator CheckMaxState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.1f);
            float dist = Vector3.Distance(playerTr.position, maxTr.position);
            if (dist <= attackDist)
            {
                nvAgent.Stop();
                _animation.clip = _maxAnim.punch;
            }
            else if (dist <= traceDist)
            {
                _animation.clip = _maxAnim.run;
                nvAgent.destination = playerTr.position;
                nvAgent.Resume();
            }
            else
            {
                _animation.clip = _maxAnim.idle;
                nvAgent.Stop();
            }
            _animation.Play();
        }
    }

    private void OnDamage(object[] _params)
    {
        CreateBloodEffect((Vector3)_params[0]);
        hp -= (int)_params[1];
        if (hp <= 0)
        {
            nvAgent.Stop();
            MaxDie();
        }
    }

    private void OnExpDamage()
    {
        foreach (SkinnedMeshRenderer skin in skinMesh)
        {
            skin.material = fireMax;
        }
        GameObject fire1 = (GameObject)Instantiate(fireEffect, firePivot.transform.position, Quaternion.identity);
        fire1.transform.parent = firePivot.transform;
        Destroy(fire1, 10.0f);
        StopAllCoroutines();
        nvAgent.Stop();
        nvAgent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
        _animation.Stop();
        expChk = true;
        MaxDie();
    }

    private void MaxDie()
    {
        StopAllCoroutines();
        isDie = true;
        _animation.Stop();
        SetRagdoll(true);
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        _gameUI.DispScore(1000);
        Destroy(gameObject, 8.0f);
    }

    private void CreateBloodEffect(Vector3 pos)
    {
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 2.0f);
        Vector3 decalPos = maxTr.position + (Vector3.up * 0.1f);
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;
        Destroy(blood2, 5.0f);
    }

    private IEnumerator WakeupRagdoll()
    {
        yield return new WaitForSeconds(3.0f);
        _animation.Stop();
        SetRagdoll(true);
    }

    private void SetRagdoll(bool isEnable)
    {
        foreach (Rigidbody _rbody in rBody)
        {
            _rbody.isKinematic = !isEnable;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
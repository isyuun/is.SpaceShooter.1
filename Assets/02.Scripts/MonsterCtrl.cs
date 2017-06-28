using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public enum MonsterState { idle, trace, attack, die };

    public MonsterState monState = MonsterState.idle;
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator _animator;
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
    public Material fireMonster;
    public Material normalMonster;
    private SkinnedMeshRenderer[] skinMesh;

    // Use this for initialization
    private void Awake()
    {
        monsterTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
        skinMesh = GetComponentsInChildren<SkinnedMeshRenderer>();
        // Awake 함수에서 코루틴 함수 호출 불가
        //StartCoroutine(this.CheckMonsterState());
        //StartCoroutine(this.MonsterAction());
    }

    private void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    private void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    private IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);
            if (dist <= attackDist) monState = MonsterState.attack;
            else if (dist <= traceDist) monState = MonsterState.trace;
            else monState = MonsterState.idle;
        }
    }

    private IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (monState)
            {
                case MonsterState.idle:
                    nvAgent.Stop();
                    _animator.SetBool("IsTrace", false);
                    break;

                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume();
                    _animator.SetBool("IsAttack", false);
                    _animator.SetBool("IsTrace", true);
                    break;

                case MonsterState.attack:
                    _animator.SetBool("IsAttack", true);
                    break;

                case MonsterState.die:
                    break;
            }
            yield return null;
        }
    }

    //private void OnCollisionEnter(Collision coll)
    //{
    //    if (coll.collider.tag == "BULLET")
    //    {
    //        CreateBloodEffect(coll.transform.position);
    //        Destroy(coll.gameObject);
    //        hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
    //        if (hp <= 0) MonsterDie();
    //        _animator.SetTrigger("IsHit");
    //    }
    //}

    private void MonsterDie()
    {
        gameObject.tag = "Untagged";
        StopAllCoroutines();
        isDie = true;
        monState = MonsterState.die;
        if (!expChk)
        {
            nvAgent.Stop();
            _animator.SetTrigger("IsDie");
        }
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
        _gameUI.DispScore(50);
        StartCoroutine(this.PushObjectPool());
    }

    private IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monState = MonsterState.idle;
        foreach (SkinnedMeshRenderer skin in skinMesh)
        {
            skin.material = normalMonster;
        }
        if (expChk)
        {
            nvAgent.enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Animator>().enabled = true;
            expChk = false;
        }
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;
        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }
        gameObject.SetActive(false);
    }

    private void CreateBloodEffect(Vector3 pos)
    {
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 2.0f);
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.1f);
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;
        Destroy(blood2, 5.0f);
    }

    private void OnPlayerDie()
    {
        StopAllCoroutines();
        nvAgent.Stop();
        _animator.SetTrigger("IsPlayerDie");
    }

    private void OnDamage(object[] _params)
    {
        // Debug.Log(string.Format("Hit ray {0} : {1}", _params[0], _params[1]));
        CreateBloodEffect((Vector3)_params[0]);
        hp -= (int)_params[1];
        if (hp <= 0 && monState != MonsterState.die) MonsterDie();
        _animator.SetTrigger("IsHit");
    }

    private void OnExpDamage()
    {
        foreach (SkinnedMeshRenderer skin in skinMesh)
        {
            skin.material = fireMonster;
        }
        GameObject fire1 = (GameObject)Instantiate(fireEffect, firePivot.transform.position, Quaternion.identity);
        fire1.transform.parent = firePivot.transform;
        Destroy(fire1, 10.0f);
        StopAllCoroutines();
        nvAgent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Animator>().enabled = false;
        expChk = true;
        StartCoroutine(MonsterExpDie());
    }

    private IEnumerator MonsterExpDie()
    {
        yield return new WaitForSeconds(10.0f);
        GetComponent<Rigidbody>().isKinematic = true;
        MonsterDie();
    }
}
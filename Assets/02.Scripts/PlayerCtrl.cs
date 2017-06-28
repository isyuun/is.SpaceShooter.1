using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float moveSpeed = 10.0f;
    public float rotSpeed = 100.0f;

    //private GameMgr _gameMgr;

    public Anim anim;
    public Animation _animation;
    public int hp = 100;
    private int initHp;
    public Image imgHpBar;

    public delegate void PlayerDieHandler();

    public static event PlayerDieHandler OnPlayerDie;

    private void Start()
    {
        initHp = hp;
        tr = GetComponent<Transform>();
        _animation = GetComponentInChildren<Animation>();
        //_gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();
        _animation.clip = anim.idle;
        _animation.Play();
    }

    private void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
        //if (v >= 0.1f) { _animation.clip = anim.runForward; _animation.Play(); }
        //else if (v <= -0.1f) { _animation.clip = anim.runBackward; _animation.Play(); }
        if (v >= 0.1f) { _animation.CrossFade(anim.runForward.name, 0.3f); }
        else if (v <= -0.1f) { _animation.CrossFade(anim.runBackward.name, 0.3f); }
        else if (h >= 0.1f) { _animation.CrossFade(anim.runRight.name, 0.3f); }
        else if (h <= -0.1f) { _animation.CrossFade(anim.runLeft.name, 0.3f); }
        else { _animation.CrossFade(anim.idle.name, 0.3f); }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "PUNCH" || coll.gameObject.tag == "MAX")
        {
            hp -= 5;
            float hpBar = (float)hp / (float)initHp;
            Color hpBarColor = Color.black;

            //if (hpBar <= 0.2f) hpBarColor = Color.red;
            //else if (hpBar <= 0.5f) hpBarColor = Color.yellow;
            //else hpBarColor = Color.green;

            if (hpBar <= 0.5f)
            {
                hpBarColor.r = 1.0f;
                hpBarColor.g = 1.0f * hpBar * 2.0f;
            }
            else
            {
                hpBarColor.g = 1.0f;
                hpBarColor.r = (1.0f - hpBar) * 2.0f;
            }

            imgHpBar.fillAmount = hpBar;
            imgHpBar.color = hpBarColor;

            Debug.Log("Player HP = " + hp.ToString());
            if (hp <= 0)
            {
                PlayerDie();
            }
        }
    }

    private void PlayerDie()
    {
        //Debug.Log("Player Die!!!");
        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        //foreach (GameObject monster in monsters)
        //{
        //    monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
        OnPlayerDie();
        //_gameMgr.isGameOver = true;
        GameMgr.instance.isGameOver = true;
    }
}
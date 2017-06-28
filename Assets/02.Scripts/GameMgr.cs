using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour {
    public Transform[] points;
    public GameObject monsterPrefab;
    public List<GameObject> monsterPool = new List<GameObject>();
    public float createTime = 2.0f;
    public int maxMonster = 10;
    public bool isGameOver = false;
    public static GameMgr instance = null;
    public float sfxVolume = 1.0f;
    public bool isSfxMute = false;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        for (int i = 0; i < maxMonster; i++)
        {
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            monster.name = "Monster_" + i.ToString();
            monster.SetActive(false);
            monsterPool.Add(monster);
        }
        if (points.Length > 0)
        {
            StartCoroutine(this.CreateMonster());
        }
	}

    IEnumerator CreateMonster()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(createTime);
            if (isGameOver) yield break;
            foreach (GameObject monster in monsterPool)
            {
                if (!monster.activeSelf)
                {
                    int idx = Random.Range(1, points.Length);
                    monster.transform.position = points[idx].position;
                    monster.SetActive(true);
                    break;
                }
            }
        }
    }

    public void PlaySfx(Vector3 pos, AudioClip sfx)
    {
        if (isSfxMute) return;
        GameObject soundObj = new GameObject("Sfx");
        soundObj.transform.position = pos;
        AudioSource _audioSource = soundObj.AddComponent<AudioSource>();
        _audioSource.clip = sfx;
        _audioSource.minDistance = 10.0f;
        _audioSource.maxDistance = 30.0f;
        _audioSource.volume = sfxVolume;
        _audioSource.Play();
        Destroy(soundObj, sfx.length);
    }
}

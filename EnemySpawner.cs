using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;

    [SerializeField]
    float spawnInterval;

    float timer = 0.0f;

    [SerializeField]
    int enemyLimit;

    GameObject[] enemies;

    AudioSource enemySpawnerAS;
    public AudioClip[] enemySpawnClips;

    [SerializeField]
    Text levelCompleteText;
    [SerializeField]
    Text timeText;

    [SerializeField]
    int maxTime;

    public PlayerController player;

    public AudioSource enemyDeathAS;
    public AudioClip[] enemyDeathClips;

    // Start is called before the first frame update
    void Start()
    {
        levelCompleteText.enabled = false;
        enemySpawnerAS = GetComponent<AudioSource>();
        //Time.timeScale = 0.0f;
        timeText.text = maxTime.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Spawn enemy
        if (timer > spawnInterval && Time.timeSinceLevelLoad < maxTime && enemies.Length < enemyLimit)
        {
            Debug.Log(GlobalVariables.enemyDied);
            GlobalVariables.enemyDied = false;
            GameObject enemyClone = Instantiate(enemyPrefab);
            enemySpawnerAS.pitch = Random.Range(0.5f, 1.5f);
            PlayRandomSpawnClip();
            enemyClone.transform.position = new Vector3(32.72f, 2.43f, -32.73f);
            timer = 0.0f;
        }

        else
        {
            timer += Time.deltaTime;
        }


        // Level over
        if(Time.timeSinceLevelLoad > maxTime)
        {
            levelCompleteText.enabled = true;
            timeText.enabled = false;
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
            }   

            if (Input.GetKey(KeyCode.Space) && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level 1"))
            {
                GameManager.instance.LevelTwo();
            }

            if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level 2"))
            {
                StartCoroutine(TransitionWait());

                if (player.health == 0)
                {
                    Debug.Log("player health = 0");
                    GameManager.instance.GameOver();
                }
                else
                {
                    Debug.Log("player still alive");
                    GameManager.instance.GameComplete();
                }
            }
        } else
        {
            updateTime(Mathf.RoundToInt(maxTime - Time.timeSinceLevelLoad));
        }

        if (GlobalVariables.enemyDied == true)
        {
            Debug.Log("enemy died, played sound");
            PlayRandomDeathClip();
            GlobalVariables.enemyDied = false;
        }
    }

    IEnumerator TransitionWait()
    {
        yield return new WaitForSeconds(5);
    }

    void updateTime(int time)
    {
        timeText.text = time.ToString("00");

        // if less than 1/2 time remaining, turn yellow
        // if less than 1/4 time remaining, turn red
        if (time * 4 < maxTime)
        {
            timeText.color = Color.red;
        }
        else if (time * 2 < maxTime)
        {
            timeText.color = Color.yellow;
        }
    }

    void PlayRandomDeathClip()
    {
        enemyDeathAS.PlayOneShot(enemyDeathClips[Random.Range(0, enemyDeathClips.Length)]);
    }

    void PlayRandomSpawnClip()
    {
        enemySpawnerAS.PlayOneShot(enemySpawnClips[Random.Range(0, enemySpawnClips.Length)]);
    }
}

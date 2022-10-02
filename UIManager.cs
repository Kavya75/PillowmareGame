using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField]
    Text scoreText;

    public int score;

    //public int totalScore = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        scoreText.text = "Score: " + GlobalVariables.totalScore.ToString();
    }

    IEnumerator ToGame()
    {
        yield return new WaitForSeconds(1);
        GameManager.instance.StartGame();
    }

    IEnumerator ToStart()
    {
        yield return new WaitForSeconds(1);
        GameManager.instance.MenuScreen();
    }

    public void AddScore()
    {
        GlobalVariables.totalScore += score;
        scoreText.text = "Score: " + GlobalVariables.totalScore.ToString();
    }

    public void YourScore()
    {
        scoreText.text = "Your Score: " + GlobalVariables.totalScore.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnowangelScoreManager : MonoBehaviour
{
    public static SnowangelScoreManager Instance { get; private set; }

    public TMP_Text scoreText;
    private int score = 0;
    private int inputIndex = 0;
    private bool gameOver = false; 

    private KeyCode[] commandSequence = { KeyCode.Q, KeyCode.E };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (gameOver) return; 

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(commandSequence[inputIndex]))
            {
                inputIndex++;
                if (inputIndex >= commandSequence.Length)
                {
                    score++;
                    inputIndex = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            {
                inputIndex = 0;
            }

            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    public void SetGameOver()
    {
        gameOver = true; 
        Debug.Log("Game Over! Scoring Disabled.");
    }
}

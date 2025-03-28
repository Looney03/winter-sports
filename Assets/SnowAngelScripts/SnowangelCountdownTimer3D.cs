using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnowangelCountdownTimer3D : MonoBehaviour
{
    public int startTime = 10;
    private float currentTime;
    public TMP_Text countdownText;
    public GameObject gameOverUI;

    private bool isGameOver = false;
    
    void Update()
    {
        if (!isGameOver)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                EndGame();
            }
            UpdateCountdownText();
        }
    }

    public void ResetTimer()
    {
        currentTime = startTime;
        UpdateCountdownText();
    }
    void UpdateCountdownText()
    {
        countdownText.text = "Time : " + Mathf.CeilToInt(currentTime).ToString();
    }

    void EndGame()
    {
        isGameOver = true;
        countdownText.text = "Game Over!";

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        if (SnowangelScoreManager.Instance != null)
        {
            SnowangelScoreManager.Instance.SetGameOver();
        }

        Time.timeScale = 0;
    }
}

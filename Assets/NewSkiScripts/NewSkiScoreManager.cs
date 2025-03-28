using UnityEngine;
using TMPro;

public class NewSkiScoreManager : MonoBehaviour
{
    public static NewSkiScoreManager Instance; 

    [Header("UI References")]
    public TMP_Text scoreText; 
    public TMP_Text timerText; 

    private int collectedPresents = 0;
    private int totalPresents = 20; 

    private float elapsedTime = 0f; 
    private bool isTimerRunning = true; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }



    private void Start()
    {
        UpdateScoreText();
        UpdateTimerText();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    public void AddScore()
    {
        collectedPresents++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Gates Passed: {collectedPresents}/{totalPresents}";
        }
        else
        {
            Debug.LogError("[ERROR] ScoreText (TMP) UI is not assigned in ScoreManager!");
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {FormatTime(elapsedTime)}";
        }
        else
        {
            Debug.LogError("[ERROR] TimerText (TMP) UI is not assigned in ScoreManager!");
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int centiseconds = Mathf.FloorToInt((time * 100) % 100);  

        return $"{minutes:D2}:{seconds:D2}:{centiseconds:D2}"; 
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public int GetScore()
    {
        return collectedPresents;
    }

    public string GetFormattedTime()
    {
        return FormatTime(elapsedTime);
    }
}

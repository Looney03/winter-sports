using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SkiScoreManager : MonoBehaviour
{
    public static SkiScoreManager Instance { get; private set; } 

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text timerText;

    private int gatesPassed = 0; 
    private int totalGates = 20; 

    private float elapsedTime = 0f;
    private bool isTimerRunning = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Invoke("FindUIElements", 0.5f); 
        ResetTimer();
        ResetScore(); 
        UpdateScoreText();
        UpdateTimerText();
        SetUIVisibility(false);
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Reloaded! Resetting Timer and Score...");
        ResetTimer();
        ResetScore(); 
        Invoke("FindUIElements", 0.5f); 
        SetUIVisibility(false);
    }

    public void AddScore()
    {
        gatesPassed++;
        UpdateScoreText();
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public float GetElapsedTime() => elapsedTime;
    public int GetScore() => gatesPassed;
    public string GetFormattedTime() => FormatTime(elapsedTime);

    private void ResetTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
        UpdateTimerText();
    }

    private void ResetScore()
    {
        gatesPassed = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Gates Passed: {gatesPassed}/{totalGates}";
        }
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            Debug.LogWarning("TimerText (TMP) is missing! Trying to find it...");
            FindUIElements();
            if (timerText == null) return; 
        }
        timerText.text = $"Time: {FormatTime(elapsedTime)}";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int centiseconds = Mathf.FloorToInt((time * 100) % 100);
        return $"{minutes:D2}:{seconds:D2}:{centiseconds:D2}";
    }

    private void FindUIElements()
    {
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        timerText = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();

        if (scoreText == null)
        {
            Debug.LogWarning("ScoreText (TMP) UI is still missing! Ensure it exists in the scene.");
        }
        if (timerText == null)
        {
            Debug.LogWarning("TimerText (TMP) UI is still missing! Ensure it exists in the scene.");
        }
    }

    public void SetUIVisibility(bool isVisible)
    {
        if (scoreText != null) scoreText.gameObject.SetActive(isVisible);
        if (timerText != null) timerText.gameObject.SetActive(isVisible);
    }
}

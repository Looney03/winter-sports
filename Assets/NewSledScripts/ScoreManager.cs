using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TMP_Text scoreText;
    private int collectedPresents = 0;
    private int totalPresents = 42;

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
        UpdateScoreText();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject foundText = GameObject.Find("ScoreText");
        if (foundText != null)
        {
            scoreText = foundText.GetComponent<TMP_Text>();
            Debug.Log("ScoreText reassigned after scene reload.");
        }
        else
        {
            Debug.LogWarning("ScoreText not found in scene after reload.");
        }

        collectedPresents = 0;
        UpdateScoreText();
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
            scoreText.text = $"Collected Presents: {collectedPresents}/{totalPresents}";
        }
        else
        {
            Debug.LogError("[ERROR] ScoreText (TMP) UI is not assigned in ScoreManager!");
        }
    }

    public int GetScore()
    {
        return collectedPresents;
    }
}

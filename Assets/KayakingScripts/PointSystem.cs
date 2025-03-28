using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PointSystem : MonoBehaviour
{
    public int score = 0;               
    public float gameTime = 60f;        
    public TextMeshProUGUI scoreText;   
    public TextMeshProUGUI timerText;   
    public TextMeshProUGUI highScoreText;
    public GameObject summaryPanel;     
    public float coinDetectionRadius = 1.5f; 
    public bool showHighScore = true;

    private bool gameActive = true;
    private float bestScore; 

    void Start()
    {
      bestScore = PlayerPrefs.GetFloat("BestScore", float.MinValue);
      UpdateHighScoreUI();

        UpdateScoreText();
        UpdateTimerText();
        summaryPanel.SetActive(false); 
    }

    void Update()
    {
        if (gameActive)
        {
            
            gameTime -= Time.deltaTime;
            UpdateTimerText();

            if (gameTime <= 0)
            {
                EndGame();
            }

            DetectAndCollectCoins();
        }
    }

    void DetectAndCollectCoins()
{
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, coinDetectionRadius);
    foreach (Collider col in hitColliders)
    {

        if (col.gameObject.tag == "Coin") 
        {
            
            score += 1;  
            UpdateScoreText();
            Destroy(col.gameObject); 
        }
        
    }
}

    void UpdateScoreText()
{
    if (scoreText != null)
    {
        scoreText.text = "Score: " + score;
        
    }
}

    void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(gameTime);
    }

    void EndGame()
    {
        gameActive = false; 
        summaryPanel.SetActive(true); 


        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetFloat("BestScore", bestScore);
            PlayerPrefs.Save();
        }


        if (showHighScore){
          summaryPanel.GetComponentInChildren<TextMeshProUGUI>().text =
              "Score: " + Mathf.Ceil(score) + "\n" +
              "High Score: " + (bestScore < float.MaxValue ? Mathf.Ceil(bestScore) : "No Record Yet");
        }
        else{
          summaryPanel.GetComponentInChildren<TextMeshProUGUI>().text =
              "Score: " + Mathf.Ceil(score);
        }

        UpdateHighScoreUI();
    }

    void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Best Score: " + (bestScore < float.MaxValue ? Mathf.Ceil(bestScore) : "No Record Yet");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}

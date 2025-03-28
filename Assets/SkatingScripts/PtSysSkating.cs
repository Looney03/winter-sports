using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PtSysSkating : MonoBehaviour
{
    public float gameTime = 0f;        
    public TextMeshProUGUI timerText;  
    public TextMeshProUGUI ringCountText;  
    public TextMeshProUGUI coinsToNextRingText; 
    public TextMeshProUGUI highScoreText;
    public GameObject summaryPanel;    
    public GameObject ringPrefab;      
    public float coinDetectionRadius = 1.5f; 
    public bool showHighScore = true;

    private bool gameActive = true;
    private int collectedCoins = 0;     
    private int collectedRings = 0;     
    public static int coinsPerRing = 5; 
    public static int maxRings = 3;     
    private float bestTime; 


    void Start()
    {
        bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        UpdateHighScoreUI();

        UpdateTimerText();
        UpdateRingUI();
        summaryPanel.SetActive(false); 
    }

    void Update()
    {
        if (gameActive)
        {
            gameTime += Time.deltaTime;
            UpdateTimerText();
            
        }
    }

    void SpawnRing()
    {
        Debug.Log("Generating Ring...");

        if (ringPrefab == null)
        {
            Debug.LogError("⚠️ No ring prefab assigned!");
            return;
        }

        Vector3 spawnPosition = GetValidRingSpawnPosition();

        if (spawnPosition != Vector3.zero)
        {
            spawnPosition.y += 5f; 
            GameObject ring = Instantiate(ringPrefab, spawnPosition, Quaternion.identity);
            ring.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            ring.transform.rotation = Quaternion.Euler(0, 0, 90);
            ring.tag = "Ring";
            ring.AddComponent<RingBehavior>(); 
            Debug.Log("🏆 Ring spawned at: " + spawnPosition);
        }
        else
        {
            Debug.LogWarning("⚠️ No valid spawn position found for the ring!");
        }
    }

    Vector3 GetValidRingSpawnPosition()
    {
        Debug.Log("Finding Valid Position...");

        float minDistance = 10f;  
        float maxDistance = 30f; 

        for (int i = 0; i < 10; i++) 
        {
            
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0; 

            Vector3 spawnPosition = transform.position + randomDirection.normalized * Random.Range(minDistance, maxDistance);
            spawnPosition.y += 10f; 

            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit, 20f))
            {
                if (hit.collider.CompareTag("Land")) 
                {
                    Debug.Log("✅ Found valid position: " + hit.point);
                    return hit.point + Vector3.up * 0.5f; 
                }
            }
        }

        Debug.Log("❌ No valid position found");
        return Vector3.zero; 
    }

    void UpdateRingUI()
    {
        if (ringCountText != null)
        {
            ringCountText.text = "Rings: " + collectedRings + " / " + maxRings;
        }

        if (coinsToNextRingText != null)
        {
            int coinsNeeded = coinsPerRing - collectedCoins;
            coinsToNextRingText.text = coinsNeeded + " Coins for Next Ring";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
          Debug.Log("Coin detected");
          collectedCoins++; 
          Destroy(other.gameObject);

          if (collectedCoins >= coinsPerRing)
          {
              collectedCoins = 0; 
              SpawnRing();
          }
          UpdateRingUI();
        }
        if (other.gameObject.CompareTag("Ring"))
        {
            Destroy(other.gameObject);
            collectedRings++;
            UpdateRingUI();

            if (collectedRings >= maxRings)
            {
                EndGame();
            }
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
        timerText.gameObject.SetActive(false);
        ringCountText.gameObject.SetActive(false);
        coinsToNextRingText.gameObject.SetActive(false);

            if (gameTime < bestTime)
            {
                bestTime = gameTime;
                PlayerPrefs.SetFloat("BestTime", bestTime);
                PlayerPrefs.Save();
            }

            if (showHighScore){
                summaryPanel.GetComponentInChildren<TextMeshProUGUI>().text =
                "Time Taken: " + Mathf.Ceil(gameTime) + " sec\n" +
                "Best Time: " + (bestTime < float.MaxValue ? Mathf.Ceil(bestTime) + " sec" : "No Record Yet");
            }

            else{
            summaryPanel.GetComponentInChildren<TextMeshProUGUI>().text =
                "Time Taken: " + Mathf.Ceil(gameTime) + " sec";
            }
            UpdateHighScoreUI();
    }

    void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Best Time: " + (bestTime < float.MaxValue ? Mathf.Ceil(bestTime) + " sec" : "No Record Yet");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

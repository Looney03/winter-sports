using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LapManager : MonoBehaviour
{
    public int totalLaps = 3; 
    [SerializeField] private TextMeshProUGUI lapText; 
    [SerializeField] private TextMeshProUGUI timerText; 
    [SerializeField] private TextMeshProUGUI bestLapText; 
    [SerializeField] private int totalCheckpoints = 1; 
    [SerializeField] private GameObject endGamePanel; 
    [SerializeField] private TextMeshProUGUI finalTimeText; 
    [SerializeField] private TextMeshProUGUI bestLapEndText; 

    private float raceTimer = 0f;
    private float lastLapTime = 0f;
    private float bestLapTime = Mathf.Infinity;

    private int currentLap = 0;
    private bool raceStarted = false;
    private int checkpointsPassed = 0; 

    private void Update()
    {
        if (raceStarted)
        {
            raceTimer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!raceStarted)
            {
                StartRace();
            }
            else
            {
                
                if (checkpointsPassed >= totalCheckpoints)
                {
                    LapCompleted();
                }
                else
                {
                    Debug.Log("Cannot complete lap yet! Checkpoints remaining.");
                }
            }
        }
    }

    private void StartRace()
    {
        raceStarted = true;
        currentLap = 1;
        lastLapTime = 0f;
        raceTimer = 0f;
        checkpointsPassed = 0;
        UpdateLapUI();
        RespawnBoosters();
        RespawnObstacles();
    }

    private void LapCompleted()
    {
        float currentLapTime = raceTimer - lastLapTime;
        lastLapTime = raceTimer;

        if (currentLapTime < bestLapTime)
        {
            bestLapTime = currentLapTime;
            UpdateBestLapUI();
        }

        if (currentLap < totalLaps)
        {
            currentLap++;
            UpdateLapUI();
            RespawnBoosters();
            RespawnObstacles();
            RespawnCheckpoints();
            checkpointsPassed = 0;
        }
        else
        {
            EndRace();
        }
    }

    private void EndRace()
    {
        raceStarted = false;
        Debug.Log("Race Finished!");

        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpeedSkatingPlayerController playerController = player.GetComponent<SpeedSkatingPlayerController>();
            if (playerController != null)
            {
                playerController.SetPlayerMovement(false); 
            }

            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector3.zero; 
                playerRb.angularVelocity = Vector3.zero; 
                playerRb.isKinematic = true; 
            }
        }

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);

            if (finalTimeText != null)
            {
                finalTimeText.text = "Final Time: " + raceTimer.ToString("F2") + "s";
            }
            if (bestLapEndText != null)
            {
                bestLapEndText.text = "Best Lap: " + (bestLapTime == Mathf.Infinity ? "N/A" : bestLapTime.ToString("F2") + "s");
            }
        }
        else
        {
            Debug.LogError("End Game Panel is not assigned in the Inspector!");
        }

        Time.timeScale = 0f;
    }

    private void RespawnBoosters()
    {
        Debug.Log("Respawning all boosters...");

        Booster[] boosterScripts = FindObjectsOfType<Booster>(true);

        if (boosterScripts.Length == 0)
        {
            Debug.LogWarning("No Booster objects found, even among inactive ones!");
            return;
        }

        foreach (Booster booster in boosterScripts)
        {
            booster.gameObject.SetActive(true);
        }
    }

    private void RespawnObstacles()
    {
        Debug.Log("Respawning all obstacles...");

        Obstacle[] obstacles = FindObjectsOfType<Obstacle>(true);

        if (obstacles.Length == 0)
        {
            Debug.LogWarning("No obstacles found!");
            return;
        }

        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.gameObject.SetActive(true);
        }
    }

    private void RespawnCheckpoints()
    {
        Debug.Log("Respawning all checkpoints...");

        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>(true);

        if (checkpoints.Length == 0)
        {
            Debug.LogWarning("No Checkpoints found!");
            return;
        }

        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.gameObject.SetActive(true);
            checkpoint.ResetCheckpoint();
        }
    }

    private void UpdateLapUI()
    {
        if (lapText != null)
        {
            lapText.text = "Lap: " + currentLap + "/" + totalLaps;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + raceTimer.ToString("F2") + "s";
        }
    }

    private void UpdateBestLapUI()
    {
        if (bestLapText != null)
        {
            bestLapText.text = "Best Lap: " + bestLapTime.ToString("F2") + "s";
        }
    }

    public void CheckpointPassed()
    {
        checkpointsPassed++;
        Debug.Log("Checkpoint passed! Total: " + checkpointsPassed);
    }

    public void RestartGame()
    {
        Debug.Log("Play Again button clicked! Restarting...");

        Time.timeScale = 1f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.isKinematic = false; 
            }

            SpeedSkatingPlayerController playerController = player.GetComponent<SpeedSkatingPlayerController>();
            if (playerController != null)
            {
                playerController.SetPlayerMovement(true); 
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

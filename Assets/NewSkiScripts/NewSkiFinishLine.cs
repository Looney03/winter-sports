using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewSkiFinishLine : MonoBehaviour
{
    public GameObject finishPanel; 
    public TMP_Text finishText; 
    public Image backgroundOverlay; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            SkiScoreManager.Instance.StopTimer(); 
            int collectedPresents = SkiScoreManager.Instance.GetScore();
            string finalTime = SkiScoreManager.Instance.GetFormattedTime();
            ShowFinishScreen(collectedPresents, finalTime);
        }
    }

    private void ShowFinishScreen(int collectedPresents, string finalTime)
    {
        if (finishPanel != null && finishText != null && backgroundOverlay != null)
        {
            finishPanel.SetActive(true);
            finishText.text = $"You have reached the finish line!\nGates Passed: {collectedPresents}\nTime: {finalTime}";
            backgroundOverlay.color = new Color(0, 0, 0, 0.5f); 
            Time.timeScale = 0; 
        }
        else
        {
            Debug.LogError("[ERROR] Finish UI elements are not assigned in FinishLine script!");
        }
    }
}

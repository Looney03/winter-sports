using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FinishLine : MonoBehaviour
{
    public GameObject finishPanel; 
    public TMP_Text finishText; 
    public Image backgroundOverlay; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            int collectedPresents = ScoreManager.Instance.GetScore();
            ShowFinishScreen(collectedPresents);
        }
    }

    private void ShowFinishScreen(int collectedPresents)
    {
        if (finishPanel != null && finishText != null && backgroundOverlay != null)
        {
            finishPanel.SetActive(true);
            finishText.text = $"You have reached the finish line!\nCollected Presents: {collectedPresents}";
            backgroundOverlay.color = new Color(0, 0, 0, 0.5f); 
            Time.timeScale = 0; 
        }
        else
        {
            Debug.LogError("[ERROR] Finish UI elements are not assigned in FinishLine script!");
        }
    }
}

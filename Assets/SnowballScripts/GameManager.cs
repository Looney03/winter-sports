using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    private int score = 0; 

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

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"[GameManager] Score: {score}");
    }

    public int GetScore()
    {
        return score;
    }
}

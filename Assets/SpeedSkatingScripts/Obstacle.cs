using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float slowdownAmount = 5f; 
    [SerializeField] private bool cancelBoost = true;   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit the obstacle!");

            SpeedSkatingPlayerController player = other.GetComponent<SpeedSkatingPlayerController>();
            if (player != null)
            {
                player.ApplySlowdown(slowdownAmount, cancelBoost);
            }

            gameObject.SetActive(false);
        }
    }
}

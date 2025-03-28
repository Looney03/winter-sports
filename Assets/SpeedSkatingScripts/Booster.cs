using UnityEngine;

public class Booster : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touched booster! Disabling booster...");
            SpeedSkatingPlayerController player = other.GetComponent<SpeedSkatingPlayerController>();
            if (player != null)
            {
                player.ActivateBoost();
            }

            gameObject.SetActive(false); 
        }
    }
}

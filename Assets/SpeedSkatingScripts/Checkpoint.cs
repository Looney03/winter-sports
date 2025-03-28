using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player passed a checkpoint!");
            FindObjectOfType<LapManager>().CheckpointPassed();
            gameObject.SetActive(false); 
        }
    }

    public void ResetCheckpoint()
    {
        gameObject.SetActive(true); 
    }
}

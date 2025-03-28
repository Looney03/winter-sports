using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAgainManager : MonoBehaviour
{
    public GameObject playAgainButton; 

    void Update()
    {
        if (playAgainButton.activeInHierarchy && Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayAgain();
        }
    }

    public void PlayAgain()
    {
        Debug.Log("Reloading Scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

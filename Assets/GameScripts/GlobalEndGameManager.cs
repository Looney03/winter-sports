using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalEndGameManager : MonoBehaviour
{
    public GameObject pausePanel; // Assign in Inspector

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Optional: auto-reassign if panel not set
        if (pausePanel == null)
        {
            GameObject found = GameObject.Find("PausePanel");
            if (found != null)
            {
                pausePanel = found;
                Debug.Log("PausePanel found automatically after scene load.");
            }
            else
            {
                Debug.LogWarning("PausePanel not found after scene load.");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Return))
        {
            ActivatePausePanel();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(LoadHomeMenuSafely());
        }
    }

    private IEnumerator LoadHomeMenuSafely()
    {
        yield return new WaitForSecondsRealtime(0.1f); // Small delay to avoid timing issues
        Time.timeScale = 1; // Resume time
        SceneManager.LoadScene("HomePage");
    }


    private void ActivatePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            Debug.Log("L Key Pressed! Pause Panel Activated.");
        }
        else
        {
            Debug.LogWarning("Pause Panel is not assigned!");
        }
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1;
        Debug.Log("Game Resumed.");
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToSelection()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SelectionMenu");
    }
}

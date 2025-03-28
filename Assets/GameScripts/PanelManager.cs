using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels;  
    public GameObject levelSelection; 
    public GameObject playButton; 
    public GameObject settingsButton; 
    public GameObject quitButton; 

    void Start()
    {
        int skipMainMenu = PlayerPrefs.GetInt("SkipMainMenu", 0);
        Debug.Log("PanelManager Start() called. SkipMainMenu value: " + skipMainMenu);

        if (skipMainMenu == 1)
        {
            Debug.Log("Returning from game, keeping Level Selection open.");
            ShowLevelSelection();
            PlayerPrefs.SetInt("SkipMainMenu", 0); 
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Normal start. Showing Main Menu.");
            ShowMainMenu();
        }
    }

    public void ShowLevelSelection()
    {
        Debug.Log("Executing ShowLevelSelection()");

        if (playButton != null) playButton.SetActive(false);
        if (settingsButton != null) settingsButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);

        if (levelSelection != null)
        {
            levelSelection.SetActive(true);
            Debug.Log("Level Selection UI is now ACTIVE.");
        }
        else
        {
            Debug.LogError("LevelSelection is NULL in PanelManager!");
        }
    }

    public void ShowMainMenu()
    {
        Debug.Log("Executing ShowMainMenu()");

        if (playButton != null) playButton.SetActive(true);
        if (settingsButton != null) settingsButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);

        if (levelSelection != null) levelSelection.SetActive(false);
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Closes the built game
#endif
    }

    public void CloseLevelSelection()
    {
        Debug.Log("Closing Level Selection...");

        if (levelSelection != null)
        {
            levelSelection.SetActive(false);
        }

        ShowMainMenu(); 
    }


}

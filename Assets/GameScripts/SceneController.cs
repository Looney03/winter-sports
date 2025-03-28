using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public GameObject backToSelectionButton; 

    void Update()
    {
        if (backToSelectionButton != null &&
        backToSelectionButton.activeInHierarchy &&
        Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadHomePage();
        }
    }

    public void LoadHomePage()
    {
        Debug.Log("Setting SkipMainMenu to 1 before returning to Homepage.");
        PlayerPrefs.SetInt("SkipMainMenu", 1);
        PlayerPrefs.Save();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("HomePage");
    }
}

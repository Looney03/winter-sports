using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelSelection : MonoBehaviour
{
    public GameLevelData[] gameLevels; 
    public Image demoImage;       
    public TextMeshProUGUI gameTitleText;
    public Image tutorialImage;   
    public TextMeshProUGUI tutorialText;
    public GameObject tutorialPanel; 
    public GameObject settingsPanel; 
    public Transform parameterContainer; 
    public GameObject parameterPrefab;
    public GameObject nextArrow;  
    public GameObject prevArrow;  
    public GameObject playButton;  


    private int currentGameIndex = 0;
    private int currentTutorialIndex = 0;
    private Vector2 playButtonOriginalPos;
    private List<GameObject> activeParams = new List<GameObject>();

    void Start()
    {
        settingsPanel.SetActive(false);
        playButtonOriginalPos = playButton.GetComponent<RectTransform>().anchoredPosition;
        LoadGameData();
    }

    public void LoadGameData()
    {
        GameLevelData game = gameLevels[currentGameIndex];

        gameTitleText.text = game.gameName;
        

        demoImage.gameObject.SetActive(true);
        demoImage.sprite = game.demoPlay;


        
        if (game.tutorialImages.Length > 0)
        {
            tutorialImage.sprite = game.tutorialImages[0]; 
        }
        tutorialText.text = game.tutorialText;
    }

    public void NextGame()
    {
        currentGameIndex = (currentGameIndex + 1) % gameLevels.Length;
        LoadGameData();
    }

    public void PreviousGame()
    {
        currentGameIndex = (currentGameIndex - 1 + gameLevels.Length) % gameLevels.Length;
        LoadGameData();
    }

    public void ShowTutorial()
    {
        currentTutorialIndex = 0;

        tutorialPanel.SetActive(true);

        playButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);

        playButton.SetActive(false); 
        DisplayTutorialPage();
    }


    private void DisplayTutorialPage()
    {
        GameLevelData game = gameLevels[currentGameIndex];

        if (game.tutorialImages.Length == 0) return;

        tutorialImage.sprite = game.tutorialImages[currentTutorialIndex];

        if (tutorialPanel.activeSelf)
        {
            bool isFirst = currentTutorialIndex == 0;
            bool isLast = currentTutorialIndex == game.tutorialImages.Length - 1;

            prevArrow.SetActive(!isFirst);
            nextArrow.SetActive(!isLast);
            playButton.SetActive(isLast);
        }
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false); 

        prevArrow.SetActive(true);
        nextArrow.SetActive(true);
        playButton.SetActive(true);

        playButton.GetComponent<RectTransform>().anchoredPosition = playButtonOriginalPos;
    }



    public void NextTutorialPage()
    {
        GameLevelData game = gameLevels[currentGameIndex];
        if (currentTutorialIndex < game.tutorialImages.Length - 1)
        {
            currentTutorialIndex++;
            DisplayTutorialPage();
        }
    }

    public void PreviousTutorialPage()
    {
        if (currentTutorialIndex > 0)
        {
            currentTutorialIndex--;
            DisplayTutorialPage();
        }
    }


    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        GenerateParameterUI();
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void OnNextButtonPressed()
    {
        if (tutorialPanel.activeSelf)
            NextTutorialPage();
        else
            NextGame();
    }

    public void OnPreviousButtonPressed()
    {
        if (tutorialPanel.activeSelf)
            PreviousTutorialPage();
        else
            PreviousGame();
    }


    public void StartGame()
    {
        settingsPanel.SetActive(false);
        GameDataBridge.currentLevelData = gameLevels[currentGameIndex];
        SceneManager.LoadScene(gameLevels[currentGameIndex].sceneName);
    }


    public void GenerateParameterUI()
    {

        foreach (GameObject obj in activeParams)
        {
            Destroy(obj);
        }
        activeParams.Clear();

        GameLevelData game = gameLevels[currentGameIndex];

        foreach (var param in game.adjustableParameters)
        {

            GameObject newParam = Instantiate(parameterPrefab, parameterContainer);
            activeParams.Add(newParam);

            RectTransform rectTransform = newParam.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0, -activeParams.Count * 50); 
            rectTransform.sizeDelta = new Vector2(500, 50); 

            TextMeshProUGUI label = newParam.transform.Find("Label").GetComponent<TextMeshProUGUI>();
            label.text = param.paramName;

            TMP_InputField floatInput = newParam.transform.Find("FloatInputField")?.GetComponent<TMP_InputField>();
            TMP_InputField intInput = newParam.transform.Find("IntInputField")?.GetComponent<TMP_InputField>();
            Toggle boolToggle = newParam.transform.Find("BoolToggle")?.GetComponent<Toggle>();
            TMP_Dropdown selectionDropdown = newParam.transform.Find("SelectionDropdown")?.GetComponent<TMP_Dropdown>();

            if (floatInput != null) floatInput.gameObject.SetActive(false);
            if (intInput != null) intInput.gameObject.SetActive(false);
            if (boolToggle != null) boolToggle.gameObject.SetActive(false);
            if (selectionDropdown != null) selectionDropdown.gameObject.SetActive(false);


            if (param.paramType == ParamType.Float)
            {
                if (floatInput != null)
                {
                    floatInput.gameObject.SetActive(true);
                    floatInput.contentType = TMP_InputField.ContentType.DecimalNumber;
                    floatInput.text = param.floatValue.ToString();
                    floatInput.onValueChanged.AddListener((string newVal) =>
                    {
                        if (float.TryParse(newVal, out float parsed))
                        {
                            param.floatValue = parsed;
                        }
                    });
                }
            }
            else if (param.paramType == ParamType.Int)
            {
                if (intInput != null)
                {
                    intInput.gameObject.SetActive(true);
                    intInput.contentType = TMP_InputField.ContentType.IntegerNumber;
                    intInput.text = param.intValue.ToString();
                    intInput.onValueChanged.AddListener((string newVal) =>
                    {
                        if (int.TryParse(newVal, out int parsed))
                        {
                            param.intValue = parsed;
                        }
                    });
                }
            }
            else if (param.paramType == ParamType.Bool)
            {
                if (boolToggle != null)
                {
                    boolToggle.gameObject.SetActive(true);
                    boolToggle.isOn = param.boolValue;
                    boolToggle.onValueChanged.AddListener((bool isOn) =>
                    {
                        param.boolValue = isOn;
                    });
                }
            }
            else if (param.paramType == ParamType.Selection)
            {
                if (selectionDropdown != null)
                {
                    selectionDropdown.gameObject.SetActive(true);
                    selectionDropdown.ClearOptions();
                    List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                    foreach (string option in param.selection)
                    {
                        options.Add(new TMP_Dropdown.OptionData(option));
                    }
                    selectionDropdown.AddOptions(options);
                    selectionDropdown.value = 0; 
                    param.intValue = 0;
                    selectionDropdown.onValueChanged.AddListener((int selectedIndex) =>
                    {
                        
                        Debug.Log(selectedIndex);
                        param.intValue = selectedIndex;

                       
                    });
                }
            }
        }
    }
}

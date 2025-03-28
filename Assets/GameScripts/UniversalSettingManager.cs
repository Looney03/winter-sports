using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

public class UniversalSettingManager : MonoBehaviour
{
    private string configPath = "Assets/StreamingAssets/MotionInput/data/config.json";

    [Header("Settings Asset")]
    public UniversalSettings universalSettings;

    [Header("UI References")]
    public GameObject commonSetPan;         
    public Transform parameterContainer;    
    public GameObject parameterPrefab;      

    
    private List<GameObject> activeParams = new List<GameObject>();

    void Start()
    {
        if(commonSetPan != null)
            commonSetPan.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        if(commonSetPan != null)
            commonSetPan.SetActive(true);
        GenerateCSParameterUI();
    }

    public void CloseSettingsPanel()
    {
        if(commonSetPan != null)
            commonSetPan.SetActive(false);
    }
    public void ApplyResolutionSetting(GameParameter resolutionParam)
    {
        int selectedIndex = resolutionParam.intValue; 
        int width = 1920, height = 1080; 
        switch (selectedIndex)
        {
            case 0: 
                width = 1920;
                height = 1080;
                break;
            case 1: 
                width = 1280;
                height = 720;
                break;
            case 2: 
                width = 800;
                height = 600;
                break;
            default:
                width = 1280;
                height = 720;
                break;
        }
        
        Screen.SetResolution(width, height, true);
    }

    public void ChangeCameraSource(int source, string configPath) 
    {
        if (File.Exists(configPath))
        {
            string originalJsonContent = File.ReadAllText(configPath);
            JObject jsonObj = JObject.Parse(originalJsonContent);
            jsonObj["camera"]["source"] = source;

            string modifiedJson = jsonObj.ToString();
            File.WriteAllText(configPath, modifiedJson);
        }
    }


    public void GenerateCSParameterUI()
    {
       
        foreach (GameObject obj in activeParams)
        {
            Destroy(obj);
        }
        activeParams.Clear();

        foreach (var param in universalSettings.universalParameters)
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
            Slider sliderBar = newParam.transform.Find("SliderBar")?.GetComponent<Slider>();
            TMP_InputField intInput = newParam.transform.Find("IntInputField")?.GetComponent<TMP_InputField>();
            Toggle boolToggle = newParam.transform.Find("BoolToggle")?.GetComponent<Toggle>();
            TMP_Dropdown selectionDropdown = newParam.transform.Find("SelectionDropdown")?.GetComponent<TMP_Dropdown>();

            if (floatInput != null) floatInput.gameObject.SetActive(false);
            if (sliderBar != null) sliderBar.gameObject.SetActive(false);
            if (intInput != null) intInput.gameObject.SetActive(false);
            if (boolToggle != null) boolToggle.gameObject.SetActive(false);
            if (selectionDropdown != null) selectionDropdown.gameObject.SetActive(false);

            if (param.paramType == ParamType.Float)
            {
                if (param.paramName == "Volume:" )
                {
                    if (sliderBar != null)
                    {
                        sliderBar.gameObject.SetActive(true);
                        sliderBar.value = param.floatValue;  
                        sliderBar.onValueChanged.RemoveAllListeners();
                        sliderBar.onValueChanged.AddListener((float newVal) =>
                        {
                            param.floatValue = newVal;
                            AudioListener.volume = newVal;
                        });
                    }
                    else if (floatInput != null)
                    {
                        floatInput.gameObject.SetActive(true);
                        floatInput.contentType = TMP_InputField.ContentType.DecimalNumber;
                        floatInput.text = param.floatValue.ToString();
                        floatInput.onValueChanged.RemoveAllListeners();
                        floatInput.onValueChanged.AddListener((string newVal) =>
                        {
                            if (float.TryParse(newVal, out float parsed))
                            {
                                param.floatValue = parsed;
                                AudioListener.volume = parsed;
                            }
                        });
                    }
                }
                else 
                {
                    if (floatInput != null)
                    {
                        floatInput.gameObject.SetActive(true);
                        floatInput.contentType = TMP_InputField.ContentType.DecimalNumber;
                        floatInput.text = param.floatValue.ToString();
                        floatInput.onValueChanged.RemoveAllListeners();
                        floatInput.onValueChanged.AddListener((string newVal) =>
                        {
                            if (float.TryParse(newVal, out float parsed))
                            {
                                param.floatValue = parsed;
                            }
                        });
                    }
                }
            }
            else if (param.paramType == ParamType.Int)
            {
                if (intInput != null)
                {
                    intInput.gameObject.SetActive(true);
                    intInput.contentType = TMP_InputField.ContentType.IntegerNumber;
                    intInput.text = param.intValue.ToString();
                    intInput.onValueChanged.RemoveAllListeners();
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
                    boolToggle.onValueChanged.RemoveAllListeners();
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

                 
                    if (options.Count > 0)
                    {
                        int storedIndex = param.intValue;
                        if (storedIndex < 0 || storedIndex >= options.Count)
                        {
                            storedIndex = 0;
                            param.intValue = 0;
                        }
                        selectionDropdown.value = storedIndex;
                    }
                    selectionDropdown.onValueChanged.RemoveAllListeners();
                    selectionDropdown.onValueChanged.AddListener((int selectedIndex) =>
                    {
                        param.intValue = selectedIndex;
 
                        if(param.paramName == "Resolution:")
                        {
                            ApplyResolutionSetting(param);
                        }
                        else if (param.paramName == "Camera Source:")
                        {
                            ChangeCameraSource(param.intValue, configPath);
                        }
                        Debug.Log("Selected option index for " + param.paramName + ": " + selectedIndex);
                    });
                }
            }
        }
    }
}

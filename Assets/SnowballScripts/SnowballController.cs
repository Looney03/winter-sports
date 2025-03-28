using UnityEngine;
using TMPro;

public class SnowballController : MonoBehaviour
{
  public Transform countdown;
  public TextMeshProUGUI timerText;
  void Start()
  {
    CountdownTimer3D timer = countdown.GetComponent<CountdownTimer3D>();
    timerText.gameObject.SetActive(true);
    timer.startTime = 40;
      // Grab the data
      GameLevelData data = GameDataBridge.currentLevelData;
      if (data == null)
      {
          Debug.LogWarning("No GameLevelData found in GameDataBridge!");
          return;
      }

      foreach (var param in data.adjustableParameters)
      {
        if (param.paramName == "Game Time:"){
          timer.startTime = param.intValue;
        } else if (param.paramName == "Show Timer:"){
          if (!param.boolValue){
            timerText.gameObject.SetActive(false);
          }
        }
      }
      timer.ResetTimer();
  }
}

using UnityEngine;
using TMPro;

public class SkateController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public Transform player;
    public GameObject mapUI;
    void Start()
  {
    timerText.gameObject.SetActive(true);
    PtSysSkating ps = player.GetComponent<PtSysSkating>();
    SkateMoveAnimator mv = player.GetComponent<SkateMoveAnimator>();
    mapUI.SetActive(false);
    ps.showHighScore = true;
      GameLevelData data = GameDataBridge.currentLevelData;
      if (data == null)
      {
          Debug.LogWarning("No GameLevelData found in GameDataBridge!");
          return;
      }

      foreach (var param in data.adjustableParameters)
      {
          if (param.paramName == "Show Timer:")
          {
                if (!param.boolValue){
                    timerText.gameObject.SetActive(false);
            }
          } else if (param.paramName == "Show High Score:"){
            if (!param.boolValue){
                ps.showHighScore = false;
            }
          } else if (param.paramName == "Coins per Ring:"){
            PtSysSkating.coinsPerRing = param.intValue;
          } else if (param.paramName == "Rings to collect:"){
            PtSysSkating.maxRings = param.intValue;
          }else if (param.paramName == "Forward Speed:")
          {
            mv.maxSpeed = 12f;
            mv.acceleration = 7f;
              switch(param.intValue)
              {
                  case 0:
                      mv.maxSpeed = 8f;
                      mv.acceleration = 4f;
                      break;
                  case 1:
                      mv.maxSpeed = 12f;
                      mv.acceleration = 7f;
                      break;
                  case 2:
                      mv.maxSpeed = 16f;
                      mv.acceleration = 11f;
                      break;
              }
          }
            else if (param.paramName == "Show Map:")
            {
                if (param.boolValue)
                {
                    mapUI.SetActive(true);
                }
            }
        }
  }
}

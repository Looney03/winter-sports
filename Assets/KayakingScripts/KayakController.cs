using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class KayakController : MonoBehaviour
{
  public Transform wood;
  public Transform blue;
  public Transform orange;
  public GameObject map;
  public GameObject mapUI;
  public CinemachineVirtualCamera myVirtualCam;
  public TextMeshProUGUI scoreText;
  public TextMeshProUGUI timerText;

  void Start()
  {
    MapFollow mapFollow = map.GetComponent<MapFollow>();
    PointSystem wps = wood.GetComponent<PointSystem>();
    BoatMovement wmv = wood.GetComponent<BoatMovement>();
    wps.showHighScore = true;
    wps.gameTime = 120f;
    wmv.moveSpeed = 7f;
    PointSystem bps = blue.GetComponent<PointSystem>();
    BoatMovement bmv = blue.GetComponent<BoatMovement>();
    bps.showHighScore = true;
    bps.gameTime = 120f;
    bmv.moveSpeed = 7f;
    PointSystem ops = orange.GetComponentInChildren<PointSystem>();
    BoatMovement omv = orange.GetComponentInChildren<BoatMovement>();
    ops.showHighScore = true;
    ops.gameTime = 120f;
    omv.moveSpeed = 7f;
    wood.gameObject.SetActive(false);
    blue.gameObject.SetActive(false);
    orange.gameObject.SetActive(false);
    timerText.gameObject.SetActive(true);
    scoreText.gameObject.SetActive(true);
    mapUI.SetActive(false);
      // Grab the data
      GameLevelData data = GameDataBridge.currentLevelData;
      if (data == null)
      {
          Debug.LogWarning("No GameLevelData found in GameDataBridge!");
          return;
      }

      foreach (var param in data.adjustableParameters)
      {
        if (param.paramName == "Kayak Model:"){
          switch (param.intValue){
            case 0:
              wood.gameObject.SetActive(true);
              myVirtualCam.Follow = wood;
              myVirtualCam.LookAt = wood;
              mapFollow.player = wood;
              break;
            case 1:
              blue.gameObject.SetActive(true);
              myVirtualCam.Follow = blue;
              myVirtualCam.LookAt = blue;
              mapFollow.player = blue;
              break;
            case 2:
              orange.gameObject.SetActive(true);
              myVirtualCam.Follow = orange;
              myVirtualCam.LookAt = orange;
              mapFollow.player = orange;
              break;
          }
        }
        else if (param.paramName == "Show Timer:"){
          if (!param.boolValue){
            timerText.gameObject.SetActive(false);
          }
        }else if (param.paramName == "Show Score:"){
          if (!param.boolValue){
            scoreText.gameObject.SetActive(false);
          }
        }else if (param.paramName == "Show High Score:"){
          if (!param.boolValue){
            wps.showHighScore = false;
            bps.showHighScore = false;
            ops.showHighScore = false;
          }
        }else if (param.paramName == "Game Time:"){
          wps.gameTime = param.intValue;
          bps.gameTime = param.intValue;
          ops.gameTime = param.intValue;
        }else if (param.paramName == "Forward Speed:")
        {
            switch(param.intValue)
            {
                case 0:
                    wmv.moveSpeed = 5f;
                    bmv.moveSpeed = 5f;
                    omv.moveSpeed = 5f;
                    break;
                case 1:
                    wmv.moveSpeed = 7f;
                    bmv.moveSpeed = 7f;
                    omv.moveSpeed = 7f;
                    break;
                case 2:
                    wmv.moveSpeed = 10f;
                    bmv.moveSpeed = 10f;
                    omv.moveSpeed = 10f;
                    break;
            }
        }else if (param.paramName == "Show Map:")
            {
                if (param.boolValue)
                {
                    mapUI.SetActive(true);
                }
            }
        }
  }
}

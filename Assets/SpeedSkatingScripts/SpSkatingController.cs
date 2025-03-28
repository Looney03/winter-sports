using UnityEngine;

public class SpSkatingController : MonoBehaviour
{
    public Transform startingLine;
    public Transform player;
    void Start()
  {
    LapManager lm = startingLine.GetComponent<LapManager>();
    lm.totalLaps = 1;
    SpeedSkatingPlayerController mv = player.GetComponent<SpeedSkatingPlayerController>();
      // Grab the data
      GameLevelData data = GameDataBridge.currentLevelData;
      if (data == null)
      {
          Debug.LogWarning("No GameLevelData found in GameDataBridge!");
          return;
      }

      foreach (var param in data.adjustableParameters)
      {
          if (param.paramName == "Number of Labs:")
          {
                lm.totalLaps = param.intValue + 1;
          } else if (param.paramName == "Forward Speed:")
          {
            mv.maxForwardSpeed = 15f;
            mv.mashSpeedIncrement = 5f;
              switch(param.intValue)
              {
                  case 0:
                      mv.maxForwardSpeed = 10f;
                      mv.mashSpeedIncrement = 3f;
                      break;
                  case 1:
                      mv.maxForwardSpeed = 15f;
                      mv.mashSpeedIncrement = 5f;
                      break;
                  case 2:
                      mv.maxForwardSpeed = 20f;
                      mv.mashSpeedIncrement = 8f;
                      break;
              }
          }
      }
  }
}

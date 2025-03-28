using UnityEngine;

public class NewSkiController : MonoBehaviour
{
    void Start()
    {

        GameLevelData data = GameDataBridge.currentLevelData;
        if (data == null)
        {
            Debug.LogWarning("No GameLevelData found in GameDataBridge!");
            return;
        }

        foreach (var param in data.adjustableParameters)
        {

        }
    }
}

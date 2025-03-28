using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGameLevel", menuName = "Game Level")]
public class GameLevelData : ScriptableObject
{
    public string gameName;        
    public Sprite demoPlay;       
    public Sprite[] tutorialImages; 
    [TextArea(3, 10)]
    public string tutorialText;    

    public string sceneName; 
    public List<GameParameter> adjustableParameters;
}

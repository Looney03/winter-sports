using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UniversalSettings", menuName = "Settings/UniversalSettings")]
public class UniversalSettings : ScriptableObject
{
    public List<GameParameter> universalParameters = new List<GameParameter>();
}

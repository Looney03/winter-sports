using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameParameter
{
    public string paramName;         // Name of the setting (e.g., "Speed")

    public ParamType paramType;      // Enum selection for type (Dropdown in Inspector)

    public float floatValue;         // Default value if float
    public int intValue;             // Default value if int
    public bool boolValue;           // Default value if bool
    public string[] selection ;
}

// Enum for selectable parameter types (Shows as a dropdown in Unity)
public enum ParamType
{
    Float,
    Int,
    Bool,
    Selection
}

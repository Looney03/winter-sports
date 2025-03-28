using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightZone : MonoBehaviour
{
    [SerializeField] private float targetHeight = 10f;
    
    public float GetTargetHeight()
    {
        return targetHeight;
    }
}

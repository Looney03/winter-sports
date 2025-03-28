using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringEffect : MonoBehaviour
{
    public Material starMaterial;
    public float flickerSpeed = 2.0f;
    public float minIntensity = 1.0f;
    public float maxIntensity = 5.0f;

    void Update()
    {
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * flickerSpeed, 1));
        starMaterial.SetColor("_EmissionColor", Color.yellow * intensity);
    }
}

using UnityEngine;

public class CoinGroupManager : MonoBehaviour
{
    public float waterHeight = 0f;       
    public float heightOffset = 0.1f;    
    public float floatAmplitude = 0.05f; 
    public float floatSpeed = 2f;        
    public float rotationSpeed = 50f;    
    public bool useDynamicWater = false; 

    void Update()
    {
        foreach (Transform coin in transform) 
        {
            float waterLevel = useDynamicWater ? GetWaveHeightAtPosition(coin.position) : waterHeight;

            float floatingOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

            coin.position = new Vector3(
                coin.position.x, 
                waterLevel + heightOffset + floatingOffset, 
                coin.position.z
            );

            coin.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
        }
    }

    float GetWaveHeightAtPosition(Vector3 position)
    {
        return Mathf.Sin(position.x * 0.5f + Time.time) * 0.5f;
    }
}
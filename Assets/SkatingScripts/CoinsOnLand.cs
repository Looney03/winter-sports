using UnityEngine;

public class CoinsOnLand : MonoBehaviour
{
    public float floatAmplitude = 0.05f; 
    public float floatSpeed = 2f;        
    public float rotationSpeed = 50f;    
    private float initialY;              

    void Start()
    {
        initialY = transform.position.y+1; 
    }
    void Update()
    {
            float floatingOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

            transform.position = new Vector3(
                transform.position.x, 
                initialY + floatingOffset, 
                transform.position.z
            );

            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
    
    }
}
using UnityEngine;
using System.Collections;

public class RingBehavior : MonoBehaviour
{
    public float floatAmplitude = 0.5f; 
    public float floatSpeed = 2f;       
    public float rotationSpeed = 50f;   
    public float lifetime = 20f;        

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, startPosition.y + floatOffset, startPosition.z);

        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
    }

}

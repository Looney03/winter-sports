using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [SerializeField] private Transform trackCentre; 
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -15f); 
    [SerializeField] private float smoothSpeed = 5f; 
    private void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(
            trackCentre.position.x, 
            trackCentre.position.y + offset.y, 
            player.position.z + offset.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(trackCentre);
    }
}

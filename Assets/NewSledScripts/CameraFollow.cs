using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public Vector3 baseOffset = new Vector3(0, 5, -10); 
    public float smoothSpeed = 0.5f; 
    private Vector3 velocity = Vector3.zero;
    private Quaternion targetRotation;
    private bool isRotating = false;
    public float rotationSpeed = 2f; 

    private float minTiltAngle = 0f;    
    private float maxTiltAngle = 40f;  
    private float flatTiltAngle = 15f; 
    private float maxSlopeAngle = 45f;  
    public float smoothTiltSpeed = 10f; 

    private float currentTiltAngle = 15f; 

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + target.rotation * baseOffset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

            float sledSlopeDegrees = target.localEulerAngles.z;

            if (sledSlopeDegrees > 180f)
            {
                sledSlopeDegrees -= 360f;
            }

            sledSlopeDegrees = Mathf.Clamp(sledSlopeDegrees, -maxSlopeAngle, maxSlopeAngle);

            float normalizedSlope = Mathf.InverseLerp(maxSlopeAngle, -maxSlopeAngle, sledSlopeDegrees);
            float targetTiltAngle = Mathf.Lerp(minTiltAngle, maxTiltAngle, normalizedSlope);

            currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTiltAngle, smoothTiltSpeed * Time.fixedDeltaTime);

            Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
            Quaternion dynamicTilt = Quaternion.Euler(currentTiltAngle, lookRotation.eulerAngles.y, 0);
            transform.rotation = dynamicTilt; 
        }
    }

    public void RotateCameraWithSled(float newYRotation, bool disableLookAt)
    {
        targetRotation = Quaternion.Euler(flatTiltAngle, 90 + newYRotation, 0);
        isRotating = true;
    }
}
using UnityEngine;

public class SkiCameraFollow : MonoBehaviour
{
    public Transform target; 
    public Vector3 baseOffset = new Vector3(0, 1, 3); 
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
            Vector3 desiredPosition = target.position + target.TransformDirection(baseOffset);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

            float sledSlopeDegrees = target.localEulerAngles.z;

            Vector3 currentOffset = transform.position - target.position;
            Debug.Log($"Camera Offset: X = {currentOffset.x}, Y = {currentOffset.y}, Z = {currentOffset.z}");

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
        baseOffset = new Vector3(0, 1, 5); 

        float cameraYRotation = newYRotation; 

        Quaternion rotationOffset = Quaternion.Euler(0, cameraYRotation - 90, 0);
        Vector3 targetOffset = rotationOffset * baseOffset;

        targetRotation = Quaternion.LookRotation(target.position - (target.position + targetOffset));

        isRotating = true;
    }



    public void AdjustCameraOffset(bool isSlowingDown)
    {
        float targetZ = isSlowingDown ? 1f : 5f; 

        baseOffset.z = Mathf.Lerp(baseOffset.z, targetZ, 3f * Time.deltaTime);
    }
}
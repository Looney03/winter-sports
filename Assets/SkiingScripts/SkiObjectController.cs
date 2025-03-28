using UnityEngine;
using System.Collections;

public class SkiObjectController : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float zMoveSpeed = 7f;
    public float tiltAngle = 15f;
    public float groundDetectionDistance = 1.5f;
    public float heightOffset = 0.5f;
    public float landingLerpDuration = 1f;
    public float spinSpeed = 360f; 
    public float fixedDescentSpeed = 2f;
    public float landingDescentSpeed = 0.5f;
    public float jumpCooldownTime = 3f;
    public float SlowedSpeed = 10f; 

    private Rigidbody rb;
    private float spinStartTime;  
    private bool isTouchingRamp = false;
    private bool isSpinning = false;
    private bool shouldSpin = false;
    private bool isAirborne = false;
    private bool isLanding = false;
    private bool isTransitioning = false;
    private float storedYRotation;
    private bool snowParticle = true;
    private float lastJumpTime = -3f;
    private bool disableSticking = false; 
    private float targetMovementAngle; 
    private Quaternion targetRotation;
    private SantaAnimation santaAnimation;

    private ParticleSystem leftTrail;
    private ParticleSystem rightTrail;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        santaAnimation = GetComponent<SantaAnimation>();

        targetMovementAngle = 0f;

        storedYRotation = transform.rotation.eulerAngles.y;
        targetRotation = transform.rotation;

        GameObject leftTrailObj = GameObject.FindWithTag("LeftTrail");
        GameObject rightTrailObj = GameObject.FindWithTag("RightTrail");

        if (leftTrailObj != null) leftTrail = leftTrailObj.GetComponent<ParticleSystem>();
        if (rightTrailObj != null) rightTrail = rightTrailObj.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        Vector3 globalFacingDirection = transform.right; 
        float globalFacingAngle = Mathf.Atan2(globalFacingDirection.x, globalFacingDirection.z) * Mathf.Rad2Deg; 

        Vector3 globalForwardDirection = rb.linearVelocity.normalized;
        float globalForwardAngle = Mathf.Atan2(globalForwardDirection.x, globalForwardDirection.z) * Mathf.Rad2Deg;

        bool touchingGround = IsTouchingGround();

        bool isSlowingDown = Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.W);

        if (santaAnimation != null)
        {
            santaAnimation.AdjustForearmRotation(isSlowingDown);
        }
        
        SkiCameraFollow cameraFollow = FindObjectOfType<SkiCameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.AdjustCameraOffset(isSlowingDown);
        }

        Vector3 movementVelocity = transform.right * moveSpeed;
        Vector3 sideVelocity = Vector3.zero;


        if (snowParticle)
    {
        if (!leftTrail.isPlaying) leftTrail?.Play();
        if (!rightTrail.isPlaying) rightTrail?.Play();
    }
    else
    {
        if (leftTrail.isPlaying) leftTrail?.Stop();
        if (rightTrail.isPlaying) rightTrail?.Stop();
    }



        rb.linearVelocity = new Vector3(movementVelocity.x + sideVelocity.x, rb.linearVelocity.y, movementVelocity.z + sideVelocity.z);

        if (Input.GetKeyDown(KeyCode.Space) && isTouchingRamp && Time.time - lastJumpTime >= jumpCooldownTime)
        {
            shouldSpin = true;
            lastJumpTime = Time.time;
        }

        if (!isAirborne && touchingGround)
        {
            AdjustToTerrain();
        }

        if (isTransitioning)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100f * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                transform.rotation = targetRotation;
                storedYRotation = targetRotation.eulerAngles.y;
                isTransitioning = false;
            }
        }

        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.W))
        {
            moveSpeed = Mathf.Max(moveSpeed - 10f * Time.deltaTime, SlowedSpeed); 
        }
        else
        {
            moveSpeed = Mathf.Min(moveSpeed + 10f * Time.deltaTime, 15f); 
        }
    }

    private void FixedUpdate()
    {

        Vector3 forwardDirection = new Vector3(
            Mathf.Cos(Mathf.Deg2Rad * -targetMovementAngle), 
            0, 
            Mathf.Sin(Mathf.Deg2Rad * targetMovementAngle)  
        );


        Vector3 sideVelocity = Vector3.zero;
        if (!isAirborne && !isTouchingRamp) 
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                sideVelocity = -transform.right * zMoveSpeed;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                sideVelocity = transform.right * zMoveSpeed;
            }
        }


        float currentVerticalVelocity = rb.linearVelocity.y;


        rb.linearVelocity = new Vector3(
            forwardDirection.x * moveSpeed + sideVelocity.x, 
            currentVerticalVelocity, 
            forwardDirection.z * moveSpeed + sideVelocity.z
        );

        if (!disableSticking)
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.2f; 
            float rayDistance = 1.5f; 

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
            {
                float targetY = hit.point.y + heightOffset;
                float currentY = transform.position.y;

              
                heightOffset = Mathf.Lerp(heightOffset, 0.1f, 5f * Time.fixedDeltaTime); 

                
                float smoothFactor = 15f; 
                float newY = Mathf.Lerp(currentY, targetY, smoothFactor * Time.fixedDeltaTime);

                transform.position = new Vector3(transform.position.x, newY, transform.position.z);

                rb.position = new Vector3(rb.position.x, Mathf.Lerp(rb.position.y, newY, 0.2f), rb.position.z);
            }
        }

        if (!isAirborne && !isSpinning)
        {
            float currentXSpeed = rb.linearVelocity.magnitude; 

            if (currentXSpeed < moveSpeed)
            {
                rb.linearVelocity = new Vector3(forwardDirection.x * moveSpeed, rb.linearVelocity.y, forwardDirection.z * moveSpeed);
            }
        }
    }


    private void AdjustToTerrain()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, groundDetectionDistance))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                Quaternion desiredRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) 
                                            * Quaternion.Euler(0f, storedYRotation, 0f);

                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 5f);
            }
        }
    }


    private bool IsTouchingGround()
    {
        return Physics.Raycast(transform.position + Vector3.up, Vector3.down, groundDetectionDistance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp"))
        {
            isTouchingRamp = true;
            disableSticking = true; 
            snowParticle = false; 

            Invoke(nameof(EnableSpin), 0.3f);
        }
        else if (collision.gameObject.CompareTag("Terrain"))
        {
            disableSticking = false; 
            isAirborne = false;
            snowParticle = true; 
        }
    }

    private void EnableSpin() 
    {
        if (isTouchingRamp) 
        {
            shouldSpin = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ramp") && isTouchingRamp)
        {
            isTouchingRamp = false;
            snowParticle = false;
            DelayedSpin();
        }
    }

    private void DelayedSpin()
    {
        if (!isTouchingRamp && shouldSpin)
        {
            shouldSpin = false;
            StartCoroutine(Perform360SpinJump());
        }
    }


    private IEnumerator Perform360SpinJump()
    {
        isAirborne = true;
        isSpinning = true;
        spinStartTime = Time.time; 

        float spinDuration = 1f; 
        float totalRotation = 0f; 
        float angularVelocity = 360f / spinDuration; 

        while (totalRotation < 360f) 
        {
            float deltaRotation = angularVelocity * Time.deltaTime; 
            totalRotation += deltaRotation;

            transform.Rotate(Vector3.right, deltaRotation, Space.Self);

            yield return null; 
        }

        transform.Rotate(Vector3.right, 360f - totalRotation, Space.Self);

        rb.angularVelocity = Vector3.zero; 
        isSpinning = false;

        while (!IsTouchingGround()) 
        {
            yield return null;
        }

        isLanding = true;
        yield return new WaitForSeconds(landingLerpDuration);
        isAirborne = false;
        isLanding = false;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DirectionTrigger"))
        {
            targetMovementAngle = -other.transform.rotation.eulerAngles.y;

            float newYRotation = other.transform.rotation.eulerAngles.y;
            targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, newYRotation + 90, transform.rotation.eulerAngles.z);
            storedYRotation = newYRotation + 90;
            isTransitioning = true;

            float currentYVelocity = rb.linearVelocity.y;

            Vector3 newDirection = new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * -targetMovementAngle),
                0,
                Mathf.Sin(Mathf.Deg2Rad * targetMovementAngle)
            ).normalized;

            StartCoroutine(SmoothDirectionTransition(newDirection, currentYVelocity));

            StartCoroutine(AdjustToTerrainSlope());

            SkiCameraFollow cameraFollow = FindObjectOfType<SkiCameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.RotateCameraWithSled(newYRotation, true);
            }
        }
        else if (other.CompareTag("Flag"))
        {
            Collider giftCollider = other.GetComponent<Collider>();
            if (giftCollider != null)
            {
                giftCollider.enabled = false; 
            }

            SkiScoreManager.Instance.AddScore();
            Destroy(other.gameObject);
        }
    }

    private IEnumerator SmoothDirectionTransition(Vector3 newDirection, float preservedYVelocity)
    {
        float transitionTime = 0.3f; 
        float elapsedTime = 0f;

        Vector3 initialVelocity = rb.linearVelocity; 

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            rb.linearVelocity = Vector3.Lerp(
                initialVelocity, 
                new Vector3(newDirection.x * moveSpeed, preservedYVelocity, newDirection.z * moveSpeed), 
                elapsedTime / transitionTime
            );

            yield return null;
        }


        rb.linearVelocity = new Vector3(newDirection.x * moveSpeed, preservedYVelocity, newDirection.z * moveSpeed);
    }


    private IEnumerator EndTransitionAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
        storedYRotation = targetRotation.eulerAngles.y;

        Vector3 newDirection = transform.right.normalized;
        float speed = rb.linearVelocity.magnitude;
        rb.linearVelocity = newDirection * speed;

        yield return StartCoroutine(AdjustToTerrainSlope());
    }


    private IEnumerator AdjustToTerrainSlope()
    {
        float adjustmentDuration = 0.3f; 
        float elapsedTime = 0f;
        Quaternion initialRotation = transform.rotation;

        Vector3 terrainNormal = GetTerrainNormal();
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, terrainNormal) * Quaternion.Euler(0f, storedYRotation, 0f);

        while (elapsedTime < adjustmentDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / adjustmentDuration);
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            yield return null;
        }

        transform.rotation = targetRotation;
    }




    private Vector3 GetTerrainNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, groundDetectionDistance * 2f))
        {
            return hit.normal;
        }
        return Vector3.up;
    }
}
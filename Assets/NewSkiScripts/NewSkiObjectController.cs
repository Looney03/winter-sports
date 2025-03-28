using UnityEngine;
using System.Collections;

public class NewSkiObjectController : MonoBehaviour
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
    private bool isTouchingNet = false;

    private bool isOverrideActive = false;
    private float overrideEndTime = 0f;
    private int overrideDirection = 0; 
    private float targetMovementAngle;
    private Quaternion targetRotation;
    private NewSkiSantaAnimation santaAnimation;

    private ParticleSystem leftTrail;
    private ParticleSystem rightTrail;


    private void Start()
    {
        ApplySpeedSetting();

        rb = GetComponent<Rigidbody>();

        santaAnimation = GetComponent<NewSkiSantaAnimation>();

        targetMovementAngle = 0f;

        storedYRotation = transform.rotation.eulerAngles.y;
        targetRotation = transform.rotation;

        GameObject leftTrailObj = GameObject.FindWithTag("LeftTrail");
        GameObject rightTrailObj = GameObject.FindWithTag("RightTrail");

        if (leftTrailObj != null) leftTrail = leftTrailObj.GetComponent<ParticleSystem>();
        if (rightTrailObj != null) rightTrail = rightTrailObj.GetComponent<ParticleSystem>();
    }

    private void ApplySpeedSetting()
    {
        GameLevelData data = GameDataBridge.currentLevelData;
        if (data == null)
        {
            Debug.LogWarning("No GameLevelData found in GameDataBridge!");
            return;
        }

        foreach (var param in data.adjustableParameters)
        {
            if (param.paramName == "Forward Speed:")
            {
                switch(param.intValue)
                {
                    case 0:
                        moveSpeed = 12f;
                        break;
                    case 1:
                        moveSpeed = 15f;
                        break;
                    case 2:
                        moveSpeed = 17f;
                        break;
                }
            }
            Debug.Log($"Speed set to: {moveSpeed}");

            if (param.paramName == "Lateral Speed:")
            {
                switch(param.intValue)
                {
                    case 0:
                        zMoveSpeed = 5f;
                        break;
                    case 1:
                        zMoveSpeed = 7f;
                        break;
                    case 2:
                        zMoveSpeed = 9f;
                        break;
                }
            }
            Debug.Log($"Speed set to: {zMoveSpeed}");
        }
    }

    private void Update()
    {
        bool touchingGround = IsTouchingGround();

        bool isSlowingDown = Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.W);

        if (santaAnimation != null)
        {
            santaAnimation.AdjustForearmRotation(isSlowingDown);
        }

        NewSkiCameraFollow cameraFollow = FindObjectOfType<NewSkiCameraFollow>();
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

        Vector3 movementVelocity = transform.right * moveSpeed;
        Vector3 sideVelocity = Vector3.zero;
        if (!isAirborne && !isTouchingRamp)
        {
            if (isOverrideActive)
            {
                sideVelocity = (overrideDirection == 1) ? transform.right * zMoveSpeed : -transform.right * zMoveSpeed;
                Debug.Log($"Override active: Moving { (overrideDirection == 1 ? "Right" : "Left") }");
            }
            else
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
        }

        rb.linearVelocity = new Vector3(movementVelocity.x + sideVelocity.x, rb.linearVelocity.y, movementVelocity.z + sideVelocity.z);

        if (isOverrideActive && Time.time >= overrideEndTime)
        {
            isOverrideActive = false;
            Debug.Log("Override ended, controls restored.");
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
        if (collision.gameObject.CompareTag("Terrain"))
        {
            disableSticking = false;
            isAirborne = false;
            snowParticle = true;
        }
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

            NewSkiCameraFollow cameraFollow = FindObjectOfType<NewSkiCameraFollow>();
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
        else if (other.CompareTag("LeftNet"))
        {
            Debug.Log("Santa hit the Left Net - Forcing Right movement for 2 seconds.");
            OverrideControls(1); 
        }
        else if (other.CompareTag("RightNet"))
        {
            Debug.Log("Santa hit the Right Net - Forcing Left movement for 2 seconds.");
            OverrideControls(-1);
        }
    }

    private void OverrideControls(int direction)
    {
        isOverrideActive = true;
        overrideDirection = direction;
        overrideEndTime = Time.time + 2f;
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
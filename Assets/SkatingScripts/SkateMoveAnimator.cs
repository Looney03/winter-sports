using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class SkateMoveAnimator : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 15f;
    public float acceleration = 10f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpSpinSpeed = 360f;
    [SerializeField] private float jumpCooldown = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Bounce Settings")]
    [SerializeField] private float minBounceForce = 5f;    
    [SerializeField] private float maxBounceForce = 30f;   
    [SerializeField] private float bounceMultiplier = 2f; 

    private Rigidbody rb;
    private Animator animator;
    private CinemachineVirtualCamera cinemachineCam;

    private float currentSpeed = 0f;
    private bool isSpinning = false;
    private bool canJump = true;
    private float jumpTimer = 0f;
    private float spinProgress = 0f;

    private Vector3 jumpDirection;
    private Quaternion startRotation;

    private Vector3 followOffset;
    private Coroutine cameraTransitionCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on this object!");
            return;
        }
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();
        if (animator != null)
            animator.applyRootMotion = false;

        cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (cinemachineCam == null)
        {
            Debug.LogWarning("CinemachineVirtualCamera not found!");
        }
        else
        {
            cinemachineCam.Follow = transform;
            cinemachineCam.LookAt = transform;
            followOffset = cinemachineCam.transform.position - transform.position;
        }
    }

    void Update()
    {
        HandleJumpCooldown();

        if (!isSpinning)
        {
            HandleMovement();
            HandleJump();
        }
    }

    void FixedUpdate()
    {
        if (isSpinning)
        {
            PerformJumpSpin();

            if (cinemachineCam != null)
            {
                float verticalInput = Input.GetAxis("Vertical");
                if (Mathf.Abs(verticalInput) > 0.1f)
                {

                    Vector3 desiredPos = transform.position + followOffset;
                    cinemachineCam.transform.position = Vector3.Lerp(cinemachineCam.transform.position, desiredPos, Time.deltaTime * 5f);
                }
                else
                {
                    Vector3 camPos = cinemachineCam.transform.position;
                    camPos.y = transform.position.y + followOffset.y;
                    cinemachineCam.transform.position = camPos;
                }
            }
        }
        else
        {
            ApplyRotationAndMovement();
        }
    }

    void HandleJumpCooldown()
    {
        if (!canJump)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
                canJump = true;
        }
    }

    void HandleMovement()
    {
        if (isSpinning)
            return;

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            float targetSpeed = maxSpeed * verticalInput;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

            Vector3 nextPosition = rb.position + transform.forward * currentSpeed * Time.deltaTime;
            if (CanMoveToPosition(nextPosition))
            {
                rb.MovePosition(nextPosition);
                animator.SetBool("Run", true);
                animator.SetBool("Stand", false);
            }
            else
            {
                ApplyBounceBack(-transform.forward);
                currentSpeed = 0f;
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            animator.SetBool("Run", false);
            animator.SetBool("Stand", true);
        }

        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            float turnAmount = horizontalInput * rotationSpeed;
            Quaternion targetRotation = Quaternion.Euler(0f, turnAmount, 0f) * transform.rotation;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f));
        }
    }

    void ApplyRotationAndMovement()
    {
        Vector3 moveDirection = transform.forward;
        Vector3 movement = moveDirection * currentSpeed * Time.fixedDeltaTime;
        Vector3 nextPosition = rb.position + movement;
        if (CanMoveToPosition(nextPosition))
            rb.MovePosition(nextPosition);
        else
            ApplyBounceBack(-moveDirection);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            jumpDirection = currentSpeed > 0.1f ? transform.forward * currentSpeed : Vector3.zero;
            startRotation = transform.rotation;


            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            canJump = false;
            jumpTimer = jumpCooldown;
            StartSpin();
        }
    }

    void StartSpin()
    {
        isSpinning = true;
        spinProgress = 0f;

        if (cinemachineCam != null)
            followOffset = cinemachineCam.transform.position - transform.position;

        if (cinemachineCam != null)
            cinemachineCam.Follow = null;
    }


    void PerformJumpSpin()
    {
        float spinAmount = jumpSpinSpeed * Time.deltaTime;
        spinProgress += spinAmount;
        transform.rotation = startRotation * Quaternion.Euler(0f, spinProgress, 0f);

        if (jumpDirection != Vector3.zero)
            rb.MovePosition(rb.position + jumpDirection * Time.deltaTime);

        if (spinProgress >= 360f)
        {
            isSpinning = false;
            transform.rotation = startRotation;
            if (cinemachineCam != null)
                cinemachineCam.Follow = transform;
        }
    }

    private bool CanMoveToPosition(Vector3 targetPosition)
    {
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(targetPosition.x, transform.position.y + 0.1f, targetPosition.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 30f))
        {
            if (hit.collider.CompareTag("Coin"))
                return true;
            if (hit.collider.CompareTag("Land") ||
                hit.collider.CompareTag("Ground") ||
                hit.collider.CompareTag("Mountain"))
                return true;
        }
        return false;
    }

    private void ApplyBounceBack(Vector3 bounceDirection)
    {
        float scaledBounceForce = Mathf.Clamp(currentSpeed * bounceMultiplier, minBounceForce, maxBounceForce);
        rb.AddForce(bounceDirection.normalized * scaledBounceForce, ForceMode.Impulse);
    }
}

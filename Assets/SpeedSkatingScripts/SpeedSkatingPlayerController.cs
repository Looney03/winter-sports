using System.Collections;
using UnityEngine;

public class SpeedSkatingPlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float forwardSpeed = 0f;
    public float maxForwardSpeed = 10f;
    [SerializeField] private float forwardSpeedDecayRate = 2f;
    public float mashSpeedIncrement = 0.5f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float boostSpeed = 15f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float collisionStopFactor = 0.5f;

    private Rigidbody rb;
    private bool aPressed = false;
    private bool dPressed = false;
    private bool isBoosted = false;
    private KeyCode lastKeyPressed = KeyCode.None;
    private bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Update()
    {
        HandleInput();
        animator.SetFloat("Speed", forwardSpeed);
    }

    private void FixedUpdate()
    {
        ProcessTranslation();
        ProcessRotation();
        if (!aPressed && !dPressed)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void HandleInput()
    {
        if (!canMove) return;

        if ((Input.GetKeyDown(KeyCode.UpArrow) && lastKeyPressed != KeyCode.UpArrow) ||
            (Input.GetKeyDown(KeyCode.DownArrow) && lastKeyPressed != KeyCode.DownArrow))
        {
            forwardSpeed += mashSpeedIncrement;
            forwardSpeed = Mathf.Clamp(forwardSpeed, 0f, maxForwardSpeed);
            lastKeyPressed = Input.GetKeyDown(KeyCode.UpArrow) ? KeyCode.UpArrow : KeyCode.DownArrow;
        }

        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !isBoosted)
        {
            forwardSpeed -= forwardSpeedDecayRate * Time.deltaTime;
            forwardSpeed = Mathf.Max(0f, forwardSpeed);
        }

        float hInput = Input.GetAxis("Horizontal");
        aPressed = hInput < 0;
        dPressed = hInput > 0;
    }

    private void ProcessTranslation()
    {
        float currentSpeed = isBoosted ? boostSpeed : forwardSpeed;
        rb.linearVelocity = transform.forward * currentSpeed;
    }


    private void ProcessRotation()
    {
        float turnDirection = 0f;
        if (aPressed && !dPressed)
        {
            turnDirection = -1f;
        }
        else if (dPressed && !aPressed)
        {
            turnDirection = 1f;
        }

        if (turnDirection != 0f)
        {
            float turnAmount = turnDirection * turnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    public void ActivateBoost()
    {
        StartCoroutine(BoostCoroutine());
    }

    private IEnumerator BoostCoroutine()
    {
        isBoosted = true;
        yield return new WaitForSeconds(boostDuration);
        isBoosted = false;
    }

    public void ApplySlowdown(float amount, bool cancelBoost)
    {
        Debug.Log("Applying slowdown...");

        forwardSpeed -= amount;
        forwardSpeed = Mathf.Max(0f, forwardSpeed);

        if (cancelBoost && isBoosted)
        {
            Debug.Log("Boost canceled by obstacle!");
            isBoosted = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            Debug.Log("Player hit the track boundary!");

            StartCoroutine(SmoothSpeedReduction(collisionStopFactor, 0.5f));
            Vector3 pushAway = collision.contacts[0].normal * 2f;
            rb.AddForce(pushAway, ForceMode.Impulse);
            rb.angularVelocity = Vector3.zero;
        }
    }

    private IEnumerator SmoothSpeedReduction(float reductionFactor, float duration)
    {
        float initialSpeed = forwardSpeed;
        float targetSpeed = initialSpeed * reductionFactor;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            forwardSpeed = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        forwardSpeed = targetSpeed;
    }
    public void SetPlayerMovement(bool enable)
    {
        canMove = enable;
        if (!enable)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


}

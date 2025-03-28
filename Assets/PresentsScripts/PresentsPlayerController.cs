using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PresentsPlayerController : MonoBehaviour
{
    [SerializeField] InputAction movement;
    [SerializeField] InputAction aButton;
    [SerializeField] InputAction dButton;
    [SerializeField] private InputAction stopButton;
    [SerializeField] float controlSpeed = 10f;
    [SerializeField] float verticalSpeed = 10f;
    public float controlForwardSpeed = 5f;

    [SerializeField] float positionPitchFactor = 5f;
    [SerializeField] float controlPitchFactor = 9f;
    [SerializeField] float positionYawFactor = -20f;
    [SerializeField] float controlRollFactor = -20f;
    [SerializeField] float lerpSpeed = 5f;
    [SerializeField] float rotationLerpSpeed = 5f;
    [SerializeField] float heightAdjustmentSpeed = 1.5f;
    [SerializeField] float heightSmoothTime = 0.8f;

    private Rigidbody rb;
    private float xThrow;
    private float yThrow;
    private bool aPressed = false;
    private bool dPressed = false;

    private float targetHeight;
    private float heightVelocity = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        targetHeight = transform.position.y;
    }

    private void OnEnable()
    {
        movement.Enable();
        aButton.Enable();
        dButton.Enable();
        stopButton.Enable();

        aButton.performed += ctx => aPressed = true;
        aButton.canceled += ctx => aPressed = false;

        dButton.performed += ctx => dPressed = true;
        dButton.canceled += ctx => dPressed = false;
    }

    private void OnDisable()
    {
        movement.Disable();
        aButton.Disable();
        dButton.Disable();
        stopButton.Disable();
    }

    void Update()
    {
        Vector2 inputVector = movement.ReadValue<Vector2>();
        xThrow = Mathf.Lerp(xThrow, inputVector.x, Time.deltaTime * lerpSpeed);
        yThrow = Mathf.Lerp(yThrow, inputVector.y, Time.deltaTime * lerpSpeed);
    }

    private void FixedUpdate()
    {
        ProcessTranslation();
        ProcessRotation();
    }

    private void ProcessRotation()
    {
        Quaternion currentRotation = rb.rotation;
        Vector3 currentEulerAngles = currentRotation.eulerAngles;

        float heightDifference = targetHeight - rb.position.y;
        float pitch = Mathf.LerpAngle(
            currentEulerAngles.z,
            heightDifference * positionPitchFactor,
            Time.deltaTime * 5f
        );

        float yaw = currentEulerAngles.y + (xThrow * positionYawFactor * Time.deltaTime);
        float targetRoll = xThrow * controlRollFactor;
        float roll = Mathf.LerpAngle(currentEulerAngles.x, targetRoll, Time.deltaTime * rotationLerpSpeed);

        Quaternion targetRotation = Quaternion.Euler(roll, yaw, pitch);
        rb.MoveRotation(targetRotation);
    }

    private void ProcessTranslation()
    {
        Debug.Log($"Stop Button Pressed: {stopButton.IsPressed()}");

        bool moveForward = !stopButton.IsPressed();
        float scaledForwardSpeed = moveForward ? controlForwardSpeed : 0f;

        Vector3 forwardMovement = transform.right * scaledForwardSpeed * Time.deltaTime;
        Vector3 horizontalMovement = -transform.forward * xThrow * controlSpeed * Time.deltaTime;
        Vector3 verticalMovement = Vector3.up * yThrow * verticalSpeed * Time.deltaTime;

        float newY = Mathf.SmoothDamp(rb.position.y, targetHeight, ref heightVelocity, heightSmoothTime);

        if (Mathf.Abs(yThrow) > 0.1f)
        {
            newY += verticalMovement.y;
        }

        Vector3 movement = forwardMovement + horizontalMovement;
        rb.MovePosition(new Vector3(rb.position.x + movement.x, newY, rb.position.z + movement.z));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HeightZone"))
        {
            HeightZone heightZone = other.GetComponent<HeightZone>();
            if (heightZone != null)
            {
                Debug.Log($"Entered Height Zone: {heightZone.name}, New Target Height: {heightZone.GetTargetHeight()}");
                targetHeight = heightZone.GetTargetHeight();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX = 100f;
    public float sensY = 100f;

    public Transform cam;
    public Transform orientation;

    private float mouseX;
    private float mouseY;

    private float multiplier = 0.01f;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ApplySensitivitySetting(); 
    }

    private void Update()
    {
        MyInput();

        cam.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        yRotation = Mathf.Clamp(yRotation, -45f, 45f);
    }

    public void SetRotation(float xInput, float yInput)
    {
        yRotation += xInput * sensX * multiplier;
        xRotation -= yInput * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        yRotation = Mathf.Clamp(yRotation, -45f, 45f);
    }

    private void ApplySensitivitySetting()
    {
        GameLevelData data = GameDataBridge.currentLevelData;
        if (data == null)
        {
            Debug.LogWarning("No GameLevelData found in GameDataBridge!");
            return;
        }

        foreach (var param in data.adjustableParameters)
        {
            if (param.paramName == "Camera Sensitivity:")
            {
                switch (param.intValue)
                {
                    case 0: // Low
                        sensX = 40f;
                        sensY = 40f;
                        break;
                    case 1: // Medium
                        sensX = 100f;
                        sensY = 100f;
                        break;
                    case 2: // High
                        sensX = 150f;
                        sensY = 150f;
                        break;
                }
                Debug.Log($"Camera sensitivity set to: sensX = {sensX}, sensY = {sensY}");
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.M;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;
    public bool isFistDetected = false;


    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(throwKey) || isFistDetected) && readyToThrow && totalThrows > 0)
        {
            Throw();
            isFistDetected = false; 
        }
    }


    public void Throw()
    {
        readyToThrow = false;

        
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position + Vector3.up * 0.5f, cam.rotation);

        
        Destroy(projectile, 1.5f);

        
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

       
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;

        
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}

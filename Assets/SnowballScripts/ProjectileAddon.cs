using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Debug.Log("[ProjectileAddon] Object will be destroyed in few seconds.");
        Destroy(gameObject, 1.7f);
    }
}

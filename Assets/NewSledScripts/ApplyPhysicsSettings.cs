using UnityEngine;

public class ApplyPhysicsSettings : MonoBehaviour
{
    void Awake()
    {
        Physics.gravity = new Vector3(0, -15f, 0);
    }
}

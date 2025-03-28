using UnityEngine;

public class RealPlayerMove : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f; 
    public float jumpHeight = 3f;
    public float dash = 5f;
    public float rotSpeed = 10f;
    public LayerMask layer;

    private Vector3 dir = Vector3.zero;
    private bool ground = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ApplySpeedSetting(); 
    }

    void Update()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        dir = dir.normalized;

        CheckGround();

        if (Input.GetButtonDown("Jump") && ground)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (dir != Vector3.zero)
        {
            Vector3 movement = dir * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.fixedDeltaTime);
        }
    }

    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 1.0f, layer))
        {
            ground = true;
        }
        else
        {
            ground = false;
        }
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
            if (param.paramName == "Speed:")
            {
                switch (param.intValue)
                {
                    case 0:
                        speed = 7f;
                        break;
                    case 1:
                        speed = 10f;
                        break;
                    case 2:
                        speed = 12f;
                        break;
                }
                Debug.Log($"Speed set to: {speed}");
            }
        }
    }
}

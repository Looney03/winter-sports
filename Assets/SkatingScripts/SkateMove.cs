using UnityEngine;

public class SkateMove : MonoBehaviour
{
    public float moveSpeed = 5f;       
    public float rotationSpeed = 100f; 
    public float jumpForce = 5f;       
    public bool isGrounded = true;    
    public float inputThreshold = 0.1f; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on this object!");
        }

        rb.freezeRotation = true; 
    }

    void Update()
    {

        float verticalInput = Input.GetAxis("Vertical"); 
        if (Mathf.Abs(verticalInput) > inputThreshold) 
        {
            Vector3 forwardMovement = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + forwardMovement); 
        }

        float horizontalInput = Input.GetAxis("Horizontal"); 
        if (Mathf.Abs(horizontalInput) > inputThreshold) 
        {
            Quaternion rotation = Quaternion.Euler(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);
            rb.MoveRotation(rb.rotation * rotation); 
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; 
        }
    }

        private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

        private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
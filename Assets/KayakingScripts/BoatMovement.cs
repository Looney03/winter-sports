using UnityEngine;
using System.Collections;

public class BoatMovement : MonoBehaviour
{
    public float moveSpeed = 5f;            
    public float glideDuration = 0.5f;      
    public float rotationSpeed = 50f;       
    public float obstacleAvoidanceRange = 5f;
    public float obstacleAvoidanceRangeSide = 3f;
    public float obstacleBounceBack = 2f;
    public float backOffset = 8f;
    public float upOffset = 1f;
    public float slideStrength = 2f;
    public float boostMultiplier = 2f;      
    public float boostDuration = 3f;        

    private bool isOnWater = true;           
    private float currentSpeed = 0f;        
    private bool isBoosted = false;         
    private bool lastKeyWasUp = false;      
    private bool canMove = false;           
    private bool isGliding = false;         

    private Vector3 lastMoveDirection = Vector3.zero; 

    void Update()
    {
        HandleAlternatingMovement();

        if (isOnWater && (canMove || isGliding))
        {
            if (canMove)
            {
                currentSpeed = isBoosted ? moveSpeed * boostMultiplier : moveSpeed;
                lastMoveDirection = -transform.right;
                StartCoroutine(GlideAfterMove());
                canMove = false;
            }
            Vector3 nextPos = transform.position + lastMoveDirection * currentSpeed * Time.deltaTime;
            if (NoObstacle(nextPos)){
              transform.position += lastMoveDirection * currentSpeed * Time.deltaTime;
            }
        }

        HandleRotation();
        DetectAndCollectRings();
    }

    void HandleAlternatingMovement()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!lastKeyWasUp)
            {
                lastKeyWasUp = true;
                canMove = true;
            }
            else
            {
                canMove = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (lastKeyWasUp)
            {
                lastKeyWasUp = false;
                canMove = true;
            }
            else
            {
                canMove = false;
            }
        }
    }

    void HandleRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator GlideAfterMove()
    {
        isGliding = true;
        float glideSpeed = currentSpeed;

        while (glideSpeed > 0)
        {
            glideSpeed -= moveSpeed * (Time.deltaTime / glideDuration);
            Vector3 nextPos = transform.position + lastMoveDirection * glideSpeed * Time.deltaTime;
            if (NoObstacle(nextPos)){
              transform.position += lastMoveDirection * glideSpeed * Time.deltaTime;
            }
            yield return null;
        }

        isGliding = false;
    }

    private bool NoObstacle(Vector3 targetPosition)
 {
     RaycastHit hit;

     
     if (Physics.Raycast(transform.position, (-transform.right - transform.forward) * obstacleAvoidanceRangeSide, out hit, 5f))
     {
         if (hit.collider != null && hit.collider.CompareTag("Land")) 
         {
             Debug.Log("Blocked by Land! Movement prevented.");
             return false; 
         }
     }

     
     if (Physics.Raycast(transform.position+transform.right*backOffset+transform.up*upOffset, -transform.right * obstacleAvoidanceRange, out hit, 5f))
     {
         if (hit.collider != null && hit.collider.CompareTag("Land"))
         {
             Debug.Log("Blocked by Land! Movement prevented.");
             return false;
         }
     }

     
     if (Physics.Raycast(transform.position, (-transform.right + transform.forward) * obstacleAvoidanceRangeSide, out hit, 5f))
     {
         if (hit.collider != null && hit.collider.CompareTag("Land"))
         {
             Debug.Log("Blocked by Land! Movement prevented.");
             return false;
         }
     }

     return true; 
 }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            isOnWater = true;
        }
        else
        {
            isOnWater = false;
            currentSpeed = 0f;
        }
    }

    void DetectAndCollectRings()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider col in hitColliders)
        {
            if (col.gameObject.CompareTag("Ring"))
            {
                StartCoroutine(ActivateSpeedBoost());
                Destroy(col.gameObject);
            }
        }
    }

    IEnumerator ActivateSpeedBoost()
    {
        if (!isBoosted)
        {
            isBoosted = true;
            yield return new WaitForSeconds(boostDuration);
            isBoosted = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (-transform.right - transform.forward) * obstacleAvoidanceRange);
        Gizmos.DrawRay(transform.position, -transform.right * obstacleAvoidanceRange);
        Gizmos.DrawRay(transform.position, (-transform.right + transform.forward) * obstacleAvoidanceRange);
    }
}

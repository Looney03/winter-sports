using UnityEngine;
using System.Collections;

public class WaterAnimalWander : MonoBehaviour
{
    public float moveSpeed = 2f; 
    public float rotationSpeed = 2f; 
    public float wanderRadius = 100f; 
    public float wanderInterval = 5f; 

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        while (true)
        {
            PickNewTarget();
            yield return new WaitForSeconds(wanderInterval);
        }
    }

    void PickNewTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-wanderRadius, wanderRadius),
            0, 
            Random.Range(-wanderRadius, wanderRadius)
        );

        targetPosition = transform.position + randomDirection;
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            {
                isMoving = false;
            }
        }
    }
}
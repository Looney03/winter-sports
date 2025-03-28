using System.Collections;
using UnityEngine;

public class WhaleSwimming : MonoBehaviour
{
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private Vector3 areaCenter = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 areaSize = new Vector3(20, 5, 20);
    [SerializeField] private Animator animator; 
    private Vector3 targetPosition;

    void Start()
    {
        SetNewTargetPosition();
        StartCoroutine(BreachRoutine()); 
    }

    void Update()
    {
        Swim();
    }

    private void Swim()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, swimSpeed * Time.deltaTime);

        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            SetNewTargetPosition();
        }
    }

    private void SetNewTargetPosition()
    {
        float randomX = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
        float randomY = Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2);
        float randomZ = Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);

        targetPosition = new Vector3(randomX, randomY, randomZ);
    }

    private IEnumerator BreachRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f)); 
            animator.SetFloat("BreachChance", Random.value); 
        }
    }
}

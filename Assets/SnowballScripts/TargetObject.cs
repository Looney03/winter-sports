using System.Collections;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Start()
    {
        originalScale = transform.localScale; 
        originalPosition = transform.position; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[TargetObject] Collision detected with: {collision.gameObject.name}");

        
        GameManager.Instance.AddScore(10);

        
        StartCoroutine(HitEffect());
    }

    private IEnumerator HitEffect()
    {
        
        float duration = 0.1f; 
        float magnitude = 0.1f; 

        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-magnitude, magnitude),
                Random.Range(-magnitude, magnitude),
                Random.Range(-magnitude, magnitude)
            );

            transform.position = originalPosition + randomOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition; 

        
        transform.localScale = originalScale * 1.18f; 
        yield return new WaitForSeconds(0.08f); 
        transform.localScale = originalScale; 
    }
}

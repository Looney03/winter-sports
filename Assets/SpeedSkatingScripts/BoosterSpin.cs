using UnityEngine;

public class BoosterSpin : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 100f; 

    private void Update()
    {
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
    }
}

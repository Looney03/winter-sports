using UnityEngine;

public class RealPlayerAnimation : MonoBehaviour
{
    public Animator animator; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("MoveLeft");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("MoveRight");
        }
    }
}

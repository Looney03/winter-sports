using UnityEngine;

public class RealPlayerCamera : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0, 10, -25); 

    void LateUpdate()
    {
        this.gameObject.transform.position = player.transform.position + offset;

        this.gameObject.transform.LookAt(player.transform.position);
    }
}

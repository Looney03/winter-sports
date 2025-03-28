using UnityEngine;

public class MapFollow : MonoBehaviour
{
    public Transform player;  
    public float heightOffset = 70f;  

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y + heightOffset, player.position.z);

            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}
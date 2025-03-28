using UnityEngine;
using UnityEditor;

public class SetChildCollidersAsTrigger : EditorWindow
{
    [MenuItem("Tools/Set Child Colliders as Trigger")]
    public static void SetNets()
    {
        GameObject[] netParents = GameObject.FindGameObjectsWithTag("Net"); // Find parent objects with "Net" tag

        if (netParents.Length == 0)
        {
            Debug.LogWarning("No parent objects with the 'Net' tag found!");
            return;
        }

        int count = 0;
        foreach (GameObject parent in netParents)
        {
            Collider[] childColliders = parent.GetComponentsInChildren<Collider>(); // Get all child colliders

            foreach (Collider col in childColliders)
            {
                col.isTrigger = true; // Set "Is Trigger"
                count++;
            }
        }

        Debug.Log($"{count} child colliders set to 'Is Trigger'.");
    }
}

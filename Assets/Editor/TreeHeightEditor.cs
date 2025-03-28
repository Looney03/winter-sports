using UnityEngine;
using UnityEditor;

public class TreeHeightEditor : MonoBehaviour
{
    [MenuItem("Tools/Fix Tree Heights")]
    static void FixTreeHeights()
    {
        GameObject parentObject = GameObject.FindWithTag("Tree"); // Find the Trees parent object
        Terrain terrain = Terrain.activeTerrain;

        if (terrain == null)
        {
            Debug.LogError("No active terrain found.");
            return;
        }

        if (parentObject == null)
        {
            Debug.LogError("No parent object with the 'Tree' tag found.");
            return;
        }

        Transform[] treeTransforms = parentObject.GetComponentsInChildren<Transform>();
        int fixedCount = 0;

        Undo.RegisterCompleteObjectUndo(parentObject, "Fix Tree Heights"); // Allow Undo

        foreach (Transform tree in treeTransforms)
        {
            if (tree == parentObject.transform) continue; // Skip parent

            // Adjust height
            Vector3 position = tree.position;
            float terrainHeight = terrain.SampleHeight(position);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(position.x, position.y + 10f, position.z), Vector3.down, out hit, Mathf.Infinity))
            {
                terrainHeight = hit.point.y;
            }

            tree.position = new Vector3(position.x, terrainHeight, position.z);
            
            // Set scale to (4,4,4)
            tree.localScale = new Vector3(4f, 4f, 4f);

            fixedCount++;
        }

        Debug.Log($"[Editor] Adjusted {fixedCount} trees to match terrain height and set scale to (4,4,4).");
        EditorUtility.SetDirty(parentObject); // Marks the object as changed
    }
}

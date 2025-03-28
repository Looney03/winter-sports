using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCoinSpawner : MonoBehaviour
{
    public GameObject objectPrefab; 
    public int minObjects = 2; 
    public int maxObjects = 20; 
    public float objectLifetime = 15f;

    private List<Transform> landPositions = new List<Transform>(); 
    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        GameObject[] lands = GameObject.FindGameObjectsWithTag("Land");
        foreach (GameObject land in lands)
        {
            landPositions.Add(land.transform);
        }

        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (true) 
    {
        if (spawnedObjects.Count < maxObjects) 
        {
            Transform land = landPositions[Random.Range(0, landPositions.Count)];

            Vector3 spawnPosition = GetRandomPositionOnLand(land);

            GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
            newObject.transform.Rotate(0, 0, 90);
            newObject.AddComponent<CoinsOnLand>();
            newObject.tag = "Coin";
            newObject.transform.localScale = new Vector3(2f, 0.5f, 2f);
            SphereCollider sphereCollider = newObject.GetComponent<SphereCollider>();
            if (sphereCollider == null)
            {
                sphereCollider = newObject.AddComponent<SphereCollider>(); 
            }
            sphereCollider.isTrigger = true;

            spawnedObjects.Add(newObject);
            StartCoroutine(DestroyAfterTime(newObject, objectLifetime));
        }

        yield return new WaitForSeconds(Random.Range(0f, 1f)); 
    }
    }

    int GetSpawnAmountBasedOnSize(Transform land)
    {
        Collider landCollider = land.GetComponent<Collider>();
        if (landCollider != null)
        {
            float landArea = landCollider.bounds.size.x * landCollider.bounds.size.z; 
            float normalizedSize = Mathf.InverseLerp(5f, 50f, landArea); 
            int objectCount = Mathf.RoundToInt(Mathf.Lerp(minObjects, maxObjects, normalizedSize)); 
            return objectCount;
        }
        return minObjects; 
    }

    Vector3 GetRandomPositionOnLand(Transform land)
    {
        Collider landCollider = land.GetComponent<Collider>();
        if (landCollider != null)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(landCollider.bounds.min.x, landCollider.bounds.max.x),
                landCollider.bounds.max.y,
                Random.Range(landCollider.bounds.min.z, landCollider.bounds.max.z)
            );
            return randomPoint;
        }
        return land.position;
    }

    IEnumerator DestroyAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            Destroy(obj);
            spawnedObjects.Remove(obj);

            ReplaceDestroyedObject();
        }
    }

    void ReplaceDestroyedObject()
{
    if (landPositions.Count == 0)
        return;

    Transform land = landPositions[Random.Range(0, landPositions.Count)];

    Vector3 spawnPosition = GetRandomPositionOnLand(land);

    GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
    newObject.transform.Rotate(0, 0, 90);
    newObject.AddComponent<CoinsOnLand>();
    newObject.tag = "Coin";
    newObject.transform.localScale = new Vector3(2f, 0.5f, 2f);
    SphereCollider sphereCollider = newObject.GetComponent<SphereCollider>();
    if (sphereCollider == null)
    {
        sphereCollider = newObject.AddComponent<SphereCollider>(); 
    }
    sphereCollider.isTrigger = true;

    spawnedObjects.Add(newObject);

    spawnedObjects.Add(newObject);
    StartCoroutine(DestroyAfterTime(newObject, objectLifetime));
}
}

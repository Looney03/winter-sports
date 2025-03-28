using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaterAnimalSpawner : MonoBehaviour
{
    public List<GameObject> animalPrefabs; 
    public int numberOfAnimals = 5;  
    public float waterSurfaceOffset = 0.2f; 
    public LayerMask waterLayer; 
    public LayerMask obstacleLayer; 

    private List<GameObject> spawnedAnimals = new List<GameObject>();

    void Start()
    {
        SpawnAnimals();
    }

    void SpawnAnimals()
    {
        int spawnedCount = 0;
        int maxAttempts = numberOfAnimals * 5; 

        while (spawnedCount < numberOfAnimals && maxAttempts > 0)
        {
            Vector3 spawnPoint = GetValidWaterPosition();
            if (spawnPoint != Vector3.zero)
            {
                GameObject animalPrefab = animalPrefabs[Random.Range(0, animalPrefabs.Count)];
                GameObject spawnedAnimal = Instantiate(animalPrefab, spawnPoint, Quaternion.identity);
                spawnedAnimal.AddComponent<WaterAnimalWander>();
                spawnedAnimals.Add(spawnedAnimal);
                spawnedCount++;
            }
            maxAttempts--;
        }
    }

    Vector3 GetValidWaterPosition()
    {
        int maxTries = 10;
        while (maxTries > 0)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(-180f, 130f),  
                50f, 
                Random.Range(-100f, 60f)  
            );

            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, waterLayer))
            {
                Vector3 waterSurfacePoint = hit.point + Vector3.up * waterSurfaceOffset;

                if (!Physics.CheckSphere(waterSurfacePoint, 1f, obstacleLayer)) 
                {
                    return waterSurfacePoint;
                }
            }
            maxTries--;
        }
        return Vector3.zero; 
    }
}
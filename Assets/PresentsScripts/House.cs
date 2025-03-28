using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] public string presentColour;
    [SerializeField] private Material outlineMaterial;

    private Material[] originalMaterials;
    private Renderer houseRenderer;
    private ParticleSystem chimneySmoke;

    public bool isDelivered {get; private set; } = false;

    void Start()
    {
        houseRenderer = GetComponent<Renderer>();

        if (houseRenderer != null)
        {
            originalMaterials = houseRenderer.sharedMaterials;
        }

        chimneySmoke = GetComponentInChildren<ParticleSystem>();
        if (chimneySmoke == null)
        {
            Debug.LogWarning($"No ParticleSystem found for {gameObject.name}.");
        }
        
    }

    public void EnableOutline()
    {
        if (houseRenderer != null && !isDelivered)
        {
            Material[] materialsWithOutline = new Material[originalMaterials.Length + 1];
            originalMaterials.CopyTo(materialsWithOutline, 0);
            materialsWithOutline[materialsWithOutline.Length - 1] = outlineMaterial;
            houseRenderer.materials = materialsWithOutline;
        }
    }

    public void DisableOutline()
    {
        if (houseRenderer != null)
        {
            houseRenderer.materials = originalMaterials;
        }
    }

    private void StopChimneySmoke()
    {
        if (chimneySmoke != null)
        {
            chimneySmoke.Stop();
            Debug.Log($"Stopped smoke for {gameObject.name}.");
        }
    }

    public void MarkAsDelivered()
    {
        if (!isDelivered)
        {
            isDelivered = true;
            DisableOutline();
            StopChimneySmoke();
        }
    }

}

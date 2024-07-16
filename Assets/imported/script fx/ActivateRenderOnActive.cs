using System.Collections.Generic;
using UnityEngine;

public class ActivateRenderOnActive : MonoBehaviour
{
    public GameObject[] chiattivo; // Lista di GameObject da controllare
    public Material coloreTemporaneo; // Materiale temporaneo per i GameObject attivi
    public string[] tagsSelezionati; // Array di tag selezionati per cui cambiare materiale

    private Dictionary<GameObject, Material> originalMaterials; // Dizionario per conservare i materiali originali

    void Start()
    {
        // Conserva i materiali originali dei GameObject con i tag selezionati
        originalMaterials = new Dictionary<GameObject, Material>();
        foreach (string tag in tagsSelezionati)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject taggedObject in taggedObjects)
            {
                Renderer renderer = taggedObject.GetComponent<Renderer>();
                if (renderer != null && !originalMaterials.ContainsKey(taggedObject))
                {
                    originalMaterials[taggedObject] = renderer.material;
                }
            }
        }
    }

    void Update()
    {
        foreach (GameObject go in chiattivo)
        {
            if (go.activeInHierarchy)
            {
                ChangeTaggedObjectMaterials(coloreTemporaneo);
                return; // Esce dal metodo una volta che trova un GameObject attivo
            }
        }

        // Se nessun GameObject è attivo, ripristina i materiali originali
        ResetTaggedObjectMaterials();
    }

    void ChangeTaggedObjectMaterials(Material newMaterial)
    {
        foreach (string tag in tagsSelezionati)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject taggedObject in taggedObjects)
            {
                Renderer renderer = taggedObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = newMaterial;
                }
            }
        }
    }

    void ResetTaggedObjectMaterials()
    {
        foreach (var kvp in originalMaterials)
        {
            Renderer renderer = kvp.Key.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = kvp.Value;
            }
        }
    }
}

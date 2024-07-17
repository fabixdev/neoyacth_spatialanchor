using System.Collections.Generic;
using UnityEngine;

public class ActivateRenderOnActive : MonoBehaviour
{
    public GameObject[] chiattivo; // Lista di GameObject da controllare
    public string[] tagsSelezionati; // Array di tag selezionati per spegnere i GameObject

    private Dictionary<GameObject, bool> originalActiveStates; // Dizionario per conservare gli stati attivi originali

    void Start()
    {
        // Conserva gli stati attivi originali dei GameObject con i tag selezionati
        originalActiveStates = new Dictionary<GameObject, bool>();
        foreach (string tag in tagsSelezionati)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject taggedObject in taggedObjects)
            {
                if (!originalActiveStates.ContainsKey(taggedObject))
                {
                    originalActiveStates[taggedObject] = taggedObject.activeSelf;
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
                DeactivateTaggedObjects();
                return; // Esce dal metodo una volta che trova un GameObject attivo
            }
        }

        // Se nessun GameObject è attivo, ripristina gli stati attivi originali
        ResetTaggedObjectActiveStates();
    }

    void DeactivateTaggedObjects()
    {
        foreach (string tag in tagsSelezionati)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject taggedObject in taggedObjects)
            {
                taggedObject.SetActive(false);
            }
        }
    }

    void ResetTaggedObjectActiveStates()
    {
        foreach (var kvp in originalActiveStates)
        {
            kvp.Key.SetActive(kvp.Value);
        }
    }
}

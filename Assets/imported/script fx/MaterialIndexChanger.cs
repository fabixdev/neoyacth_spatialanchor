// fabio vangi feat chatgpt Copyright 2023

//script per cambiare il singolo elemento dai mesh render
//scegli prima il numero dell'elemento
//inserisci il numero di materiali da cambiare
//crea i bottoni e trascina il game object nella funzione bottone
//seleziona change material e numero

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialIndexChanger : MonoBehaviour
{
    public int MaterialIndexToChange = 0; // Numero dell'indice del materiale da cambiare

    public Material[] BodyColorMat; // Il materiale da applicare al secondo elemento
    public MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ApplySecondMaterial();

        meshRenderer = GetComponent<MeshRenderer>();
        SetMaterial(0); // Usiamo l'indice 0 poiché non abbiamo più la variabile "Index"
    }

    // Funzione per cambiare il materiale al secondo elemento
    public void ApplySecondMaterial()
    {
        if (BodyColorMat != null)
        {
            if (meshRenderer.materials.Length >= MaterialIndexToChange + 1)
            {
                Material[] materials = meshRenderer.materials;
                materials[MaterialIndexToChange] = BodyColorMat[0]; // Usiamo l'indice 0 poiché non abbiamo più la variabile "Index"
                meshRenderer.materials = materials;
            }
        }
    }

    public void ChangeMaterial(int index)
    {
        if (index >= 0 && index < BodyColorMat.Length)
        {
            SetMaterial(index); // Usiamo direttamente la funzione SetMaterial invece della variabile "Index"
        }
    }

    private void SetMaterial(int index)
    {
        Material[] materials = meshRenderer.materials;
        materials[MaterialIndexToChange] = BodyColorMat[index];
        meshRenderer.materials = materials;
    }
}


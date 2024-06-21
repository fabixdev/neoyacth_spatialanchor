using System.Collections.Generic;
using UnityEngine;

public class MeshRendererController : MonoBehaviour
{
    public List<MeshRendererUpdater> listaChiVieneCambiato; // La lista degli script che verranno notificati

    public MeshRenderer partenzaMeshRenderer;

    private Color previousColor;

    void Start()
    {
        if (partenzaMeshRenderer != null)
        {
            previousColor = partenzaMeshRenderer.material.color;
        }
    }

    void Update()
    {
        if (partenzaMeshRenderer != null)
        {
            Color currentColor = partenzaMeshRenderer.material.color; // Assume che il colore sia preso dal primo materiale
            if (currentColor != previousColor)
            {
                NotificaCambiamento(currentColor);
                previousColor = currentColor;
            }
        }
    }

    void NotificaCambiamento(Color newColor)
    {
        foreach (MeshRendererUpdater updater in listaChiVieneCambiato)
        {
            if (updater != null)
            {
                updater.AggiornaColore(newColor);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Un elemento nella lista è nullo.");
            }
        }
    }
}

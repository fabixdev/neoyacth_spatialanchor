//gestore interuttori in base se un game object è acceso o spento

using System.Diagnostics;
using UnityEngine;

public class ToggleGameObjects : MonoBehaviour
{
    // Oggetti di gioco da assegnare tramite l'Inspector
    public GameObject GameDaToogle;
    public GameObject InteruttoreAcceso;
    public GameObject InteruttoreSpento;

    void Update()
    {
        // Controlla se le variabili sono assegnate
        if (GameDaToogle == null || InteruttoreAcceso == null || InteruttoreSpento == null)
        {
            //Debug.LogWarning("Uno o più GameObject non sono assegnati nell'Inspector.");
            return;
        }

        // Controlla se GameDaToogle è attivo
        if (GameDaToogle.activeSelf)
        {
            // Se GameDaToogle è attivo, attiva InteruttoreAcceso e disattiva InteruttoreSpento
            InteruttoreAcceso.SetActive(true);
            InteruttoreSpento.SetActive(false);
        }
        else
        {
            // Se GameDaToogle è disattivo, disattiva InteruttoreAcceso e attiva InteruttoreSpento
            InteruttoreAcceso.SetActive(false);
            InteruttoreSpento.SetActive(true);
        }
    }
}

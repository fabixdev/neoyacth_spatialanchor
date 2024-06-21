using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosizioneSpostatoMultiplo : MonoBehaviour
{
    public Transform[] posizioni; // Lista delle posizioni
    public float GapSpostamento; // Defining GapSpostamento

    private bool isOriginalPosition = true;

    public void PosizioneOrigine()
    {
        ApplicaPosizioneEScala(posizioni[0].position); // Assuming the original position is the first in the array
    }

    private void ApplicaPosizioneEScala(Vector3 posizione)
    {
        // Apply the position with GapSpostamento added to the y-axis
        Vector3 nuovaPosizione = new Vector3(posizione.x, posizione.y + GapSpostamento, posizione.z);
        transform.position = nuovaPosizione;
    }

    public void TogglePosizione(int index)
    {
        if (index >= 0 && index < posizioni.Length)
        {
            ApplicaPosizioneEScala(posizioni[index].position);
        }

        isOriginalPosition = !isOriginalPosition;
    }
}

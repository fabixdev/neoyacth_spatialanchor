using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosizioneScalaSpostato : MonoBehaviour
{
    public Transform PosizioneOriginale;
    public float Scala1_1 = 0.2f;

    public Transform PosizioneAggiornata;
    public float Scala1_50 = 1f;

    private bool isOriginalPosition = true;

    public void PosizioneOrigine()
    {
        // Cambia la posizione del GameObject a PosizioneOriginale con Scala1:1
        ApplicaPosizioneEscala(PosizioneOriginale.position, Scala1_1);
    }

    public void PosizioneNuova()
    {
        // Cambia la posizione del GameObject a PosizioneAggiornata con Scala1:50
        ApplicaPosizioneEscala(PosizioneAggiornata.position, Scala1_50);
    }

    private void ApplicaPosizioneEscala(Vector3 posizione, float scala)
    {
        transform.position = posizione;
        transform.localScale = new Vector3(scala, scala, scala);
    }

    public void TogglePosizione()
    {
        if (isOriginalPosition)
        {
            PosizioneNuova();
        }
        else
        {
            PosizioneOrigine();
        }

        isOriginalPosition = !isOriginalPosition;
    }
}

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ColorChanger : MonoBehaviourPunCallbacks
{
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    // Metodo per cambiare colore localmente e sincronizzare il cambio
    public void ChangeColor(Color newColor)
    {
        photonView.RPC("RPC_ChangeColor", RpcTarget.AllBuffered, newColor.r, newColor.g, newColor.b);
    }

    // RPC per sincronizzare il colore tra i client
    [PunRPC]
    void RPC_ChangeColor(float r, float g, float b)
    {
        Color color = new Color(r, g, b);
        objectRenderer.material.color = color;
    }
}

using Photon.Pun;
using UnityEngine;

public class ToggleGameObject : MonoBehaviourPun
{
    // Metodo per accendere il GameObject
    [PunRPC]
    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    // Metodo per spegnere il GameObject
    [PunRPC]
    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    // Metodo per cambiare lo stato del GameObject
    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            photonView.RPC("TurnOff", RpcTarget.AllBuffered);
        }
        else
        {
            photonView.RPC("TurnOn", RpcTarget.AllBuffered);
        }
    }
}
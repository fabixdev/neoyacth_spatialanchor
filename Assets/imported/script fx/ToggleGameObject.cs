using Photon.Pun;
using UnityEngine;

public class ToggleGameObject : MonoBehaviourPun
{
    // Metodo per accendere il GameObject
    [PunRPC]
    public void TurnOn()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOn called");
        gameObject.SetActive(true);
    }

    // Metodo per spegnere il GameObject
    [PunRPC]
    public void TurnOff()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOff called");
        gameObject.SetActive(false);
    }

    // Metodo per cambiare lo stato del GameObject
    public void Toggle()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: Toggle called");
        if (gameObject.activeSelf)
        {
            UnityEngine.Debug.Log($"{gameObject.name}: GameObject is active, turning off");
            photonView.RPC("TurnOff", RpcTarget.AllBuffered);
        }
        else
        {
            UnityEngine.Debug.Log($"{gameObject.name}: GameObject is inactive, turning on");
            photonView.RPC("TurnOn", RpcTarget.AllBuffered);
        }
    }

    // Metodo di test per chiamare Toggle
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            UnityEngine.Debug.Log($"{gameObject.name}: 'T' key pressed, calling Toggle");
            Toggle();
        }
    }
}

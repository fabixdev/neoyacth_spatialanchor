using UnityEngine;

public class ToggleGameObjectlocal : MonoBehaviour
{
    // Metodo per accendere il GameObject
    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    // Metodo per spegnere il GameObject
    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    // Metodo per cambiare lo stato del GameObject
    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }
}

using UnityEngine;

public class DisableChildren : MonoBehaviour
{
    // Questo metodo disattiva tutti i figli del GameObject a cui è attaccato questo script
    public void DisableAllChildren()
    {
        // Ottieni tutti i figli del GameObject
        foreach (Transform child in transform)
        {
            // Disattiva ogni figlio
            child.gameObject.SetActive(false);
        }
    }

    // Metodo di esempio per richiamare la disattivazione da un altro script o evento
    void Update()
    {
        // Per esempio, disattiva tutti i figli premendo il tasto 'D'
        if (Input.GetKeyDown(KeyCode.D))
        {
            DisableAllChildren();
        }
    }
}

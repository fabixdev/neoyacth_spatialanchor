using UnityEngine;

public class ApplyMaterialByTag: MonoBehaviour
{
    public string[] tagsSelezionati; // Array di tag selezionati
    public Material nuovoMateriale; // Materiale da applicare

    public void Resetto()
    {
        // Cicla attraverso ogni tag selezionato
        foreach (string tag in tagsSelezionati)
        {
            // Trova tutti i GameObject con il tag corrente
            GameObject[] oggettiConTag = GameObject.FindGameObjectsWithTag(tag);

            // Cicla attraverso ogni GameObject trovato e applica il materiale
            foreach (GameObject oggetto in oggettiConTag)
            {
                Renderer renderer = oggetto.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = nuovoMateriale;
                }
            }
        }
    }
}

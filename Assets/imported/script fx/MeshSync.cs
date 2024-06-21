using Photon.Pun;
using UnityEngine;

public class MeshSync : MonoBehaviourPun
{
    public MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Metodo per aggiornare il mesh e inviare la modifica al server
    [PunRPC]
    public void UpdateMesh()
    {
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.mesh;
    }

    // Chiamata locale per applicare il cambiamento e sincronizzarlo
    public void ApplyMeshChange()
    {
        // Applica la modifica localmente
        UpdateMesh();

        // Invia la modifica al server e agli altri client
        photonView.RPC("UpdateMesh", RpcTarget.Others);
    }
}

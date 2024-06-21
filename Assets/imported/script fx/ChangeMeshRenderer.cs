using UnityEngine;
using Photon.Pun;

public class ChangeMeshRenderer : MonoBehaviour
{
    public Mesh newMesh;
    public Material newMaterial;
    public GameObject[] objectsToChange;

    void Start()
    {
        if (objectsToChange != null && newMesh != null && newMaterial != null)
        {
            foreach (GameObject obj in objectsToChange)
            {
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    meshFilter.mesh = newMesh;
                }

                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.material = newMaterial;
                }
            }
        }
    }

    [PunRPC]
    private void TransferMaterial(int targetViewID, string materialName, int sourceViewID)
    {
        // Find the target object using its PhotonView ID
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView == null) return;
        GameObject targetObject = targetPhotonView.gameObject;

        // Find the source object using its PhotonView ID
        PhotonView sourcePhotonView = PhotonView.Find(sourceViewID);
        if (sourcePhotonView == null) return;
        GameObject sourceObject = sourcePhotonView.gameObject;

        if (targetObject != null && sourceObject != null)
        {
            // Get the MeshRenderer of the target object
            MeshRenderer targetMeshRenderer = targetObject.GetComponent<MeshRenderer>();
            if (targetMeshRenderer == null) return;

            // Find the material by name in the source object's MeshRenderer
            MeshRenderer sourceMeshRenderer = sourceObject.GetComponent<MeshRenderer>();
            if (sourceMeshRenderer == null) return;

            Material sourceMaterial = null;

            foreach (Material mat in sourceMeshRenderer.materials)
            {
                if (mat.name == materialName)
                {
                    sourceMaterial = mat;
                    break;
                }
            }

            if (sourceMaterial != null)
            {
                // Assign the material to the target object's MeshRenderer
                targetMeshRenderer.material = sourceMaterial;
                UnityEngine.Debug.Log("Material " + materialName + " assigned to target object: " + targetObject.name);
            }
        }
    }
}

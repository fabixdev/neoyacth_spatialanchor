using UnityEngine;

public class MeshRendererUpdater : MonoBehaviour
{
    public MeshRenderer targetMeshRenderer;

    public void AggiornaColore(Color newColor)
    {
        if (targetMeshRenderer != null)
        {
            targetMeshRenderer.material.color = newColor;
        }
        else
        {
            UnityEngine.Debug.LogWarning("targetMeshRenderer non è assegnato.");
        }
    }
}

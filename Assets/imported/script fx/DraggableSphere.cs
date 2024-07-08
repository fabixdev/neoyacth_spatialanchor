using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class DraggableSphere : MonoBehaviourPunCallbacks
{
    public GameObject ChiDaIlColore;
    public string tagName = ""; // Tag per gli oggetti di interesse
    public string excludedObjectTags = ""; // Tag per oggetti esclusi
    public AudioSource audioSource;

    private void Start()
    {
        if (ChiDaIlColore != null && !string.IsNullOrEmpty(tagName))
        {
            ChiDaIlColore.tag = tagName;
            UnityEngine.Debug.Log("Tag assigned to ChiDaIlColore: " + tagName);
        }
        else
        {
            UnityEngine.Debug.Log("Tag assignment failed. Ensure ChiDaIlColore is set and tagName is not empty.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ChiDaIlColore != null)
        {
            MeshRenderer sourceMeshRenderer = ChiDaIlColore.GetComponent<MeshRenderer>();

            if (sourceMeshRenderer != null)
            {
                if (other.gameObject.CompareTag(tagName) && !IsExcluded(other.gameObject))
                {
                    UnityEngine.Debug.Log("Collided with object having tag: " + tagName);

                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                    else
                    {
                        UnityEngine.Debug.Log("AudioSource is not set.");
                    }

                    MeshRenderer targetMeshRenderer = other.gameObject.GetComponent<MeshRenderer>();

                    if (targetMeshRenderer != null)
                    {
                        PhotonView otherPhotonView = other.gameObject.GetComponent<PhotonView>();

                        if (otherPhotonView != null)
                        {
                            photonView.RPC("RequestMaterialTransfer", RpcTarget.MasterClient, otherPhotonView.ViewID, sourceMeshRenderer.material.name);
                        }
                        else
                        {
                            UnityEngine.Debug.Log("The collided object does not have a PhotonView component.");
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.Log("The GameObject " + other.gameObject.name + " does not have a MeshRenderer.");
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("Collided object does not have the specified tag or is excluded: " + other.gameObject.name);
                }
            }
            else
            {
                UnityEngine.Debug.Log("The ChiDaIlColore does not have a MeshRenderer.");
            }
        }
        else
        {
            UnityEngine.Debug.Log("The ChiDaIlColore is not set.");
        }
    }

    private bool IsExcluded(GameObject obj)
    {
        string[] excludedTags = excludedObjectTags.Split(',');

        foreach (string excludedTag in excludedTags)
        {
            if (obj.CompareTag(excludedTag.Trim()))
            {
                return true;
            }
        }
        return false;
    }

    [PunRPC]
    private void RequestMaterialTransfer(int targetViewID, string materialName)
    {
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);

        if (targetPhotonView != null)
        {
            photonView.RPC("TransferMaterial", RpcTarget.AllBuffered, targetViewID, materialName);
        }
        else
        {
            UnityEngine.Debug.Log("Could not find the PhotonView with ID: " + targetViewID);
        }
    }

    [PunRPC]
    private void TransferMaterial(int targetViewID, string materialName)
    {
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);

        if (targetPhotonView != null)
        {
            GameObject targetObject = targetPhotonView.gameObject;

            if (targetObject != null && targetObject.CompareTag(tagName) && !IsExcluded(targetObject))
            {
                MeshRenderer targetMeshRenderer = targetObject.GetComponent<MeshRenderer>();
                MeshRenderer sourceMeshRenderer = ChiDaIlColore.GetComponent<MeshRenderer>();
                Material sourceMaterial = null;

                foreach (Material mat in sourceMeshRenderer.materials)
                {
                    if (mat.name == materialName)
                    {
                        sourceMaterial = mat;
                        break;
                    }
                }

                if (sourceMaterial != null && targetMeshRenderer != null)
                {
                    targetMeshRenderer.material = sourceMaterial;
                    UnityEngine.Debug.Log("Material " + materialName + " assigned to target object: " + targetObject.name);
                }
                else
                {
                    UnityEngine.Debug.Log("Material or target MeshRenderer is not valid.");
                }
            }
            else
            {
                UnityEngine.Debug.Log("Target object is in the excluded list or does not have the specified tag: " + targetObject.name);
            }
        }
        else
        {
            UnityEngine.Debug.Log("The target object is not valid.");
        }
    }
}

using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class DraggableSphere : MonoBehaviourPunCallbacks
{
    public GameObject ChiDaIlColore;
    public string tagName = "";
    public string excludedTag = "";
    public AudioSource audioSource;

    private void Start()
    {
        if (ChiDaIlColore != null && !string.IsNullOrEmpty(tagName))
        {
            if (!ChiDaIlColore.CompareTag(excludedTag))
            {
                ChiDaIlColore.tag = tagName;
                UnityEngine.Debug.Log("Tag assigned to ChiDaIlColore: " + tagName);
            }
            else
            {
                UnityEngine.Debug.Log("ChiDaIlColore has the excluded tag and its tag was not changed.");
            }
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
                if (other.gameObject.CompareTag(tagName) && !other.gameObject.CompareTag(excludedTag))
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
                    UnityEngine.Debug.Log("Collided object does not have the specified tag or is excluded: " + other.gameObject.tag);
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

    [PunRPC]
    private void RequestMaterialTransfer(int targetViewID, string materialName)
    {
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);

        if (targetPhotonView != null)
        {
            photonView.RPC("TransferMaterial", RpcTarget.AllBuffered, targetViewID, materialName);
            photonView.RPC("TransferMaterialToAllWithTag", RpcTarget.AllBuffered, materialName);
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

            if (targetObject != null && !targetObject.CompareTag(excludedTag))
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
                UnityEngine.Debug.Log("Target object is in the excluded list or has the excluded tag: " + targetObject.name);
            }
        }
        else
        {
            UnityEngine.Debug.Log("The target object is not valid.");
        }
    }

    [PunRPC]
    private void TransferMaterialToAllWithTag(string materialName)
    {
        MeshRenderer sourceMeshRenderer = ChiDaIlColore.GetComponent<MeshRenderer>();
        Material sourceMaterial = null;

        if (sourceMeshRenderer != null)
        {
            foreach (Material mat in sourceMeshRenderer.materials)
            {
                if (mat.name == materialName)
                {
                    sourceMaterial = mat;
                    break;
                }
            }
        }

        if (sourceMaterial != null)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tagName);

            foreach (GameObject obj in taggedObjects)
            {
                if (!obj.CompareTag(excludedTag))
                {
                    MeshRenderer targetMeshRenderer = obj.GetComponent<MeshRenderer>();

                    if (targetMeshRenderer != null)
                    {
                        targetMeshRenderer.material = sourceMaterial;
                        UnityEngine.Debug.Log("Material " + materialName + " assigned to object with tag: " + obj.name);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("The GameObject " + obj.name + " does not have a MeshRenderer.");
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("Object is in the excluded list or has the excluded tag: " + obj.name);
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("Source material is not valid.");
        }
    }
}

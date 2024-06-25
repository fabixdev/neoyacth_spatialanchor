using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class DraggableSphere : MonoBehaviourPunCallbacks
{
    // Reference to the GameObject with the MeshRenderer whose material will be transferred
    public GameObject sourceObject;
    public string tagName = ""; // The tag to assign and use for material change
    public List<GameObject> excludedObjects; // List of objects to exclude from material change

    // Reference to the AudioSource for playing sound effects
    public AudioSource audioSource;

    private void Start()
    {
        // Initialize the excluded objects list if it's not set
        if (excludedObjects == null)
        {
            excludedObjects = new List<GameObject>();
        }

        // Ensure the sourceObject has the specified tag
        if (sourceObject != null && !string.IsNullOrEmpty(tagName))
        {
            sourceObject.tag = tagName;
            UnityEngine.Debug.Log("Tag assigned to sourceObject: " + tagName);
        }
        else
        {
            UnityEngine.Debug.Log("Tag assignment failed. Ensure sourceObject is set and tagName is not empty.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            // Ensure the sourceObject has a MeshRenderer
            if (sourceObject != null)
            {
                MeshRenderer sourceMeshRenderer = sourceObject.GetComponent<MeshRenderer>();

                if (sourceMeshRenderer != null)
                {
                    // Check if the collided object has the specified tag and is not in the excluded list
                    if (other.gameObject.CompareTag(tagName) && !excludedObjects.Contains(other.gameObject))
                    {
                        UnityEngine.Debug.Log("Collided with object having tag: " + tagName);

                        // Play the sound effect if audioSource is set
                        if (audioSource != null)
                        {
                            audioSource.Play();
                        }
                        else
                        {
                            UnityEngine.Debug.Log("AudioSource is not set.");
                        }

                        // Get the MeshRenderer of the object we collided with
                        MeshRenderer targetMeshRenderer = other.gameObject.GetComponent<MeshRenderer>();

                        if (targetMeshRenderer != null)
                        {
                            PhotonView otherPhotonView = other.gameObject.GetComponent<PhotonView>();

                            if (otherPhotonView != null)
                            {
                                // Transfer the material from sourceObject to targetObject
                                photonView.RPC("TransferMaterial", RpcTarget.AllBuffered, otherPhotonView.ViewID, sourceMeshRenderer.material.name);

                                // Print a debug message to verify the transfer
                                UnityEngine.Debug.Log("Material transferred from " + sourceObject.name + " to " + other.gameObject.name);

                                // Transfer the material to all objects with the specified tag, excluding the ones in the list
                                photonView.RPC("TransferMaterialToAllWithTag", RpcTarget.AllBuffered, sourceMeshRenderer.material.name);
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
                        UnityEngine.Debug.Log("Collided object does not have the specified tag or is excluded: " + tagName);
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("The sourceObject does not have a MeshRenderer.");
                }
            }
            else
            {
                UnityEngine.Debug.Log("The sourceObject is not set.");
            }
        }
    }

    [PunRPC]
    private void TransferMaterial(int targetViewID, string materialName)
    {
        // Find the target object using its PhotonView ID
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);

        if (targetPhotonView != null)
        {
            GameObject targetObject = targetPhotonView.gameObject;

            if (targetObject != null)
            {
                // Check if the target object is in the excluded list
                if (!excludedObjects.Contains(targetObject))
                {
                    // Get the MeshRenderer of the target object
                    MeshRenderer targetMeshRenderer = targetObject.GetComponent<MeshRenderer>();

                    // Find the material by name in the source object's MeshRenderer
                    MeshRenderer sourceMeshRenderer = sourceObject.GetComponent<MeshRenderer>();
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
                        // Assign the material to the target object's MeshRenderer
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
                    UnityEngine.Debug.Log("Target object is in the excluded list: " + targetObject.name);
                }
            }
            else
            {
                UnityEngine.Debug.Log("The target object is not valid.");
            }
        }
        else
        {
            UnityEngine.Debug.Log("Could not find the PhotonView with ID: " + targetViewID);
        }
    }

    [PunRPC]
    private void TransferMaterialToAllWithTag(string materialName)
    {
        // Find the material by name in the source object's MeshRenderer
        MeshRenderer sourceMeshRenderer = sourceObject.GetComponent<MeshRenderer>();
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
            // Find all objects with the specified tag
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tagName);

            foreach (GameObject obj in taggedObjects)
            {
                // Check if the object is in the excluded list
                if (!excludedObjects.Contains(obj))
                {
                    MeshRenderer targetMeshRenderer = obj.GetComponent<MeshRenderer>();

                    if (targetMeshRenderer != null)
                    {
                        // Assign the material to the target object's MeshRenderer
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
                    UnityEngine.Debug.Log("Object is in the excluded list: " + obj.name);
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("Source material is not valid.");
        }
    }
}

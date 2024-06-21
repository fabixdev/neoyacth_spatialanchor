using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class handmenu : MonoBehaviour
{
    [SerializeField]
    public Transform referencePoint;

    [SerializeField]
    private TextMeshProUGUI renderStyleText;

    private void Start()
    {
        transform.parent = referencePoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (renderStyleText != null)
        {
            renderStyleText.text = "Render: " + CoLocatedPassthroughManager.Instance.visualization.ToString();
        }
        //ToggleRoomButtons(false);
    }

}

using UnityEngine;

public class DisableAtStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Disable this GameObject
        gameObject.SetActive(false);
    }
}

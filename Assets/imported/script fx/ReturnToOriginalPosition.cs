using System.Collections;
using UnityEngine;

public class ReturnToOriginalPosition : MonoBehaviour
{
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public Rigidbody rb;
    public bool isReturning = false;
    public float timer = 0f;
    public float delay = 2f;
    public float minVelocity = 0.01f;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Controlla se l'oggetto si sta muovendo
        if (rb.velocity.magnitude > minVelocity)
        {
            // Se l'oggetto si sta muovendo, ferma il ritorno e reimposta il timer
            if (isReturning)
            {
                StopCoroutine(ReturnToPosition());
                isReturning = false;
            }
            timer = 0f;
        }
        else
        {
            // Se l'oggetto non si muove ed è fuori dalla posizione originale, incrementa il timer
            if (transform.position != originalPosition && !isReturning)
            {
                timer += Time.deltaTime;
                if (timer >= delay)
                {
                    // Se il timer supera il ritardo, avvia la coroutine per il ritorno
                    isReturning = true;
                    StartCoroutine(ReturnToPosition());
                }
            }
        }
    }

    private IEnumerator ReturnToPosition()
    {
        float elapsedTime = 0f;
        float duration = 1f; // Durata della transizione

        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, originalPosition, (elapsedTime / duration));
            transform.rotation = Quaternion.Lerp(startingRot, originalRotation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isReturning = false; // Permetti all'oggetto di aggiornarsi di nuovo
        timer = 0f; // Reimposta il timer dopo il ritorno alla posizione originale
    }
}

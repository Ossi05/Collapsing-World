using UnityEngine;

public class ObjectShaker : MonoBehaviour {
    [Header("Shake Settings")]
    [SerializeField] float shakeMagnitude = 0.08f;

    Vector3 originalPosition;
    bool isShaking = false;

    void Update()
    {
        if (isShaking)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;
        }
    }

    public void StartShake()
    {
        originalPosition = transform.localPosition;
        isShaking = true;
    }

    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = originalPosition;
    }
}

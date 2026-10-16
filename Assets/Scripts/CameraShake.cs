using System.Collections;
using UnityEngine;

// Goes on Main Camera. [STEP 9 - Critical FX]
public class CameraShake : MonoBehaviour
{
    Vector3 origin;
    Coroutine routine;

    void Awake() => origin = transform.localPosition;

    public void Shake(float duration, float magnitude)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Run(duration, magnitude));
    }

    IEnumerator Run(float duration, float magnitude)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float fade = 1f - t / duration;
            transform.localPosition = origin + (Vector3)(Random.insideUnitCircle * magnitude * fade);
            yield return null;
        }
        transform.localPosition = origin;
        routine = null;
    }
}

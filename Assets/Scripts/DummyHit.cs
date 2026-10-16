using System.Collections;
using UnityEngine;

// Goes on TargetDummy. Plays the HIT animation frames and squashes for a moment.
public class DummyHit : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite idle;
    public Sprite[] hitFrames;      // HIT_0 ... HIT_6
    public float frameTime = 0.035f;

    Vector3 baseScale;
    Coroutine routine;

    void Awake() => baseScale = transform.localScale;

    public void Hit(bool crit)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Flash(crit));
    }

    IEnumerator Flash(bool crit)
    {
        transform.localScale = baseScale * (crit ? 1.25f : 1.1f);
        foreach (var frame in hitFrames)
        {
            sr.sprite = frame;
            yield return new WaitForSeconds(frameTime);
        }
        sr.sprite = idle;
        transform.localScale = baseScale;
        routine = null;
    }
}

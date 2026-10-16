using UnityEngine;
using TMPro;

// Goes on the DamageNumber prefab. Root object has a "TextMeshPro - Text" (3D, NOT the UI one).
// Each feature is a bool toggle so you can switch it on/off live in the Inspector (like the short).
// Type/paste each [STEP n] block when you reach that step in the video.
public class DamageNumber : MonoBehaviour
{
    [SerializeField] TextMeshPro text;

    // [STEP 3 - baseline]
    public float lifetime = 0.9f;

    // [STEP 4 - Pop]
    public bool pop = false;
    public float popTime = 0.2f;

    // [STEP 5 - Fade Out]
    public bool fadeOut = false;
    [Range(0f, 1f)] public float fadeStart = 0.6f;

    // [STEP 5 - Rise]
    public bool rise = false;
    public float riseSpeed = 0.5f;

    // [STEP 6 - Arc + Offset]
    public bool arcOffset = false;
    public float sideSpeed = 0.15f;
    public float gravity = 0.5f;

    // [STEP 7 - Size = Damage]
    public bool sizeByDamage = false;
    public float minSize = 0.8f;
    public float maxSize = 2f;
    public int maxDamageForSize = 200;

    // [STEP 8 - Color over time]  (set the gradient in the Inspector: white -> yellow -> orange)
    public bool colorOverTime = false;
    public Gradient colorGradient;

    // [STEP 9 - Critical FX]
    public bool criticalFx = false;
    public Color critColor = new Color(1f, 0.85f, 0.2f);
    public float critSizeMultiplier = 1.5f;

    float t;
    Vector3 velocity;
    bool isCrit;

    void Awake()
    {
        if (!text) text = GetComponent<TextMeshPro>();
    }

    public void Setup(int damage, bool crit)
    {
        // [STEP 9]
        isCrit = crit && criticalFx;
        text.text = isCrit ? damage + "!" : damage.ToString();

        // [STEP 7]
        float size = 1.2f;
        if (sizeByDamage)
            size = Mathf.Lerp(minSize, maxSize, Mathf.InverseLerp(0, maxDamageForSize, damage));
        if (isCrit) size *= critSizeMultiplier;
        text.fontSize = size;

        // [STEP 5 + 6]
        velocity = new Vector3(
            arcOffset ? Random.Range(-sideSpeed, sideSpeed) : 0f,
            rise ? riseSpeed : 0f,
            0f);
    }

    void Update()
    {
        t += Time.deltaTime;
        float n = t / lifetime;                       // 0 -> 1 over the lifetime
        if (n >= 1f) { Destroy(gameObject); return; }

        // [STEP 5 + 6] movement (gravity turns the straight rise into an arc)
        if (arcOffset && rise) velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // [STEP 4] pop
        float s = pop ? EaseOutBack(Mathf.Clamp01(t / popTime)) : 1f;
        transform.localScale = Vector3.one * s;

        // [STEP 8 + 9] color
        Color c = Color.white;
        if (isCrit) c = Color.Lerp(critColor, new Color(1f, 0.5f, 0.1f), colorOverTime ? n : 0f);
        else if (colorOverTime) c = colorGradient.Evaluate(n);

        // [STEP 5] fade
        c.a = (fadeOut && n > fadeStart) ? 1f - (n - fadeStart) / (1f - fadeStart) : 1f;
        text.color = c;
    }

    // [STEP 4] overshoots then settles = the "pop"
    static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
    }

    // [STEP 10 - Stacking] called by the spawner when a newer number appears
    public void PushUp(float amount) => transform.position += Vector3.up * amount;
}


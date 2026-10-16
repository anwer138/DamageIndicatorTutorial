using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Goes on the GameManager object. Left click or Space = hit the dummy.
public class DamageSpawner : MonoBehaviour
{
    public DamageNumber prefab;
    public Transform spawnPoint;
    public DummyHit dummy;          // [STEP 3]
    public CameraShake cameraShake; // [STEP 9]

    [Header("Damage")]
    public int minDamage = 5;
    public int maxDamage = 100;

    // [STEP 9 - Critical FX]
    [Header("Critical")]
    public bool criticalFx = false;
    [Range(0f, 1f)] public float critChance = 0.2f;
    public int critMin = 150;
    public int critMax = 400;

    // [STEP 10 - Stacking]
    [Header("Stacking")]
    public bool stacking = false;
    public float stackStep = 0.06f;

    // [STEP 11 - Spawn Offsets]
    [Header("Spawn Offsets")]
    public bool spawnOffsets = false;
    public float offsetRange = 0.08f;

    readonly List<DamageNumber> live = new List<DamageNumber>();

    void Update()
    {
        bool click = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool space = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        if (click || space) Hit();
    }

    void Hit()
    {
        bool crit = criticalFx && Random.value < critChance;
        int damage = crit ? Random.Range(critMin, critMax + 1) : Random.Range(minDamage, maxDamage + 1);

        Vector3 pos = spawnPoint.position;
        if (spawnOffsets)
            pos += new Vector3(Random.Range(-offsetRange, offsetRange), Random.Range(-offsetRange * 0.5f, offsetRange * 0.5f), 0f);

        if (stacking)
        {
            live.RemoveAll(d => d == null);
            foreach (var d in live) d.PushUp(stackStep);
        }

        DamageNumber number = Instantiate(prefab, pos, Quaternion.identity);
        number.Setup(damage, crit);
        live.Add(number);

        dummy.Hit(crit);
        if (crit && cameraShake) cameraShake.Shake(0.2f, 0.02f);
    }
}


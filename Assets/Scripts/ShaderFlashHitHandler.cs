using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class ShaderFlashHitHandler : MonoBehaviour, HitHandler
{
    [SerializeField] Color hurtColor = Color.red;
    [SerializeField] float flashStrength = 0.7f;
    [SerializeField] float totalFlashTime = 0.35f;
    [SerializeField] int numFlashes = 3;
    
    Material material;

    public void Awake() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        material = spriteRenderer.material;
        if (!material.HasProperty("_Color_Activated")) {
            Debug.LogError("ShaderFlashDamageHandler: Material does not have _Color_Activated property");
        }
    }

    private void SetColored(bool value) {
        if (value) {
            material.SetColor("_Color", hurtColor);
            material.SetFloat("_Color_Strength", flashStrength);
        }

        material.SetInt("_Color_Activated", value ? 1 : 0);
    }

    public void OnDisable() {
        SetColored(false);
    }

    private IEnumerator Flash() {
        int flashesOnAndOff = 2 * numFlashes - 1;
        float singleFlashTime = totalFlashTime / (float)flashesOnAndOff;
        for (int i = 0; i < numFlashes; i++) {
            SetColored(true);
            yield return new WaitForSeconds(singleFlashTime);
            SetColored(false);
            if (i < numFlashes - 1) yield return new WaitForSeconds(singleFlashTime);
        }
    }

    public void Hit(Vector2 force, float damage = 1)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(Flash());
        }
    }
}
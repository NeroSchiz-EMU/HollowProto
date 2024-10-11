using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeToBlack : MonoBehaviour
{
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void StartFadeToBlack(float fadeTime)
    {
        StartCoroutine(FadeToBlackRoutine(fadeTime));
    }

    IEnumerator FadeToBlackRoutine(float fadeTime)
    {
        float startTime = Time.time;

        float progress = (Time.time - startTime) / fadeTime;

        while (progress < 1f)
        {
            progress = (Time.time - startTime) / fadeTime;

            Color newFadeColor = new Color(0f, 0f, 0f, progress);

            sprite.color = newFadeColor;

            yield return null;
        }

        sprite.color = new Color(0f, 0f, 0f, 1f);
    }

    public void ResetFadeToBlack()
    {
        sprite.color = new Color(0f, 0f, 0f, 0f);
    }
}

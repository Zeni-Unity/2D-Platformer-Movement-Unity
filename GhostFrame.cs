using System.Collections;
using UnityEngine;

public class GhostFrame : MonoBehaviour
{
    public float deathTime = 0.25f;
    public bool enableDeathTime = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (enableDeathTime) StartCoroutine(FadeToZero(deathTime));
    }

    private IEnumerator FadeToZero(float duration)
    {
        float startAlpha = spriteRenderer.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float normalizedTime = elapsed / duration;

            Color newColor = spriteRenderer.color;
            newColor.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
            spriteRenderer.color = newColor;

            yield return null;
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;
        Destroy(gameObject);
    }
}

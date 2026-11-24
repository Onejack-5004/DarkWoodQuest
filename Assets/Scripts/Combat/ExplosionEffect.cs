
using UnityEngine;
using System.Collections;

public class ExplosionEffect : MonoBehaviour
{
    public float duration = 0.4f;
    public float startSize = 0.5f;
    public float endSize = 2f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(PlayExplosion());
    }

    private IEnumerator PlayExplosion()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float progress = timer / duration;

            float size = Mathf.Lerp(
                startSize,
                endSize,
                progress
            );

            transform.localScale = new Vector3(
                size,
                size,
                1f
            );

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f - progress;
                spriteRenderer.color = color;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}

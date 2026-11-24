
using UnityEngine;
using System.Collections;

public class HitscanAttack : MonoBehaviour
{
    [Header("Firing Point")]
    public Transform firePoint;

    [Header("Hit Detection")]
    public LayerMask monsterLayer;
    public LayerMask bossLayer;

    [Header("Tracer Effect")]
    public LineRenderer tracer;
    public float tracerDuration = 0.05f;
    public float tracerWidth = 0.04f;

    private SpriteRenderer spriteRenderer;
    private Coroutine tracerCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (tracer == null)
            tracer = GetComponent<LineRenderer>();

        if (tracer != null)
        {
            tracer.positionCount = 2;
            tracer.useWorldSpace = true;
            tracer.startWidth = tracerWidth;
            tracer.endWidth = tracerWidth;
            tracer.startColor = Color.yellow;
            tracer.endColor = Color.yellow;
            tracer.enabled = false;

            if (spriteRenderer != null)
            {
                tracer.sortingLayerID =
                    spriteRenderer.sortingLayerID;

                tracer.sortingOrder =
                    spriteRenderer.sortingOrder + 1;
            }
        }
    }

    public void Fire(int damage, float range)
    {
        if (damage <= 0 || range <= 0)
            return;

        float direction = 1f;

        if (spriteRenderer != null && spriteRenderer.flipX)
            direction = -1f;

        Vector2 origin = transform.position;

        if (firePoint != null)
        {
            Vector3 localPosition =
                transform.InverseTransformPoint(firePoint.position);

            localPosition.x =
                Mathf.Abs(localPosition.x) * direction;

            origin = transform.TransformPoint(localPosition);
        }

        Vector2 shootDirection =
            new Vector2(direction, 0f);

        LayerMask targetLayer = monsterLayer | bossLayer;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            shootDirection,
            range,
            targetLayer
        );

        Vector2 endPoint =
            origin + shootDirection * range;

        if (hit.collider != null)
        {
            endPoint = hit.point;

            MonsterHealth monster =
                hit.collider.GetComponentInParent<MonsterHealth>();

            if (monster != null)
            {
                monster.TakeDamage(damage);
            }

            BossHealth boss =
                hit.collider.GetComponentInParent<BossHealth>();

            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
        }

        if (tracer != null)
        {
            if (tracerCoroutine != null)
                StopCoroutine(tracerCoroutine);

            tracer.SetPosition(0, origin);
            tracer.SetPosition(1, endPoint);
            tracer.enabled = true;

            tracerCoroutine = StartCoroutine(HideTracer());
        }
    }

    private IEnumerator HideTracer()
    {
        yield return new WaitForSeconds(tracerDuration);

        tracer.enabled = false;
        tracerCoroutine = null;
    }
}

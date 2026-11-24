
using UnityEngine;
using System.Collections;

public class BossDashAttack : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 12f;
    public float dashDuration = 1.5f;
    public float stopDistance = 0.8f;

    [Header("Attack Settings")]
    public float attackWindup = 0.6f;
    public float hitRange = 1.5f;
    public float verticalHitRange = 1f;
    public int damage = 30;

    private Rigidbody2D rb;
    private Collider2D bossCollider;

    private bool cancelRequested = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        bossCollider = GetComponent<Collider2D>();

        if (bossCollider == null)
            bossCollider = GetComponentInChildren<Collider2D>();
    }

    public void CancelDash()
    {
        cancelRequested = true;
        StopHorizontalMovement();
    }

    public IEnumerator PerformDash(GameObject target)
    {
        cancelRequested = false;

        if (!IsTargetAlive(target))
            yield break;

        Debug.Log("BOSS HORIZONTAL DASH START!");

        float elapsedTime = 0f;
        float fixedY = transform.position.y;

        while (elapsedTime < dashDuration)
        {
            if (cancelRequested || !IsTargetAlive(target))
                break;

            float currentX = rb != null
                ? rb.position.x
                : transform.position.x;

            float targetX = target.transform.position.x;
            float distanceX = Mathf.Abs(targetX - currentX);

            if (distanceX <= stopDistance)
                break;

            float directionX = Mathf.Sign(targetX - currentX);

            float nextX = Mathf.MoveTowards(
                currentX,
                targetX - directionX * stopDistance,
                dashSpeed * Time.fixedDeltaTime
            );

            Vector2 nextPosition = new Vector2(nextX, fixedY);

            if (rb != null)
                rb.MovePosition(nextPosition);
            else
                transform.position = nextPosition;

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        StopHorizontalMovement();

        if (cancelRequested || !IsTargetAlive(target))
            yield break;

        Debug.Log("BOSS DASH ARRIVED!");
        Debug.Log("BOSS PREPARING TO SLASH!");

        yield return new WaitForSeconds(attackWindup);

        if (cancelRequested || !IsTargetAlive(target))
        {
            Debug.Log("BOSS DASH CANCELLED!");
            yield break;
        }

        Collider2D playerCollider =
            target.GetComponent<Collider2D>();

        if (playerCollider == null)
            playerCollider = target.GetComponentInChildren<Collider2D>();

        if (bossCollider == null || playerCollider == null)
        {
            Debug.LogWarning("Boss or Player Collider is missing!");
            yield break;
        }

        // ตรวจระยะห่างจากขอบ Collider แทน Pivot
        Bounds bossBounds = bossCollider.bounds;
        Bounds playerBounds = playerCollider.bounds;

        float horizontalGap = Mathf.Max(
            0f,
            Mathf.Max(
                bossBounds.min.x - playerBounds.max.x,
                playerBounds.min.x - bossBounds.max.x
            )
        );

        float verticalGap = Mathf.Max(
            0f,
            Mathf.Max(
                bossBounds.min.y - playerBounds.max.y,
                playerBounds.min.y - bossBounds.max.y
            )
        );

        if (horizontalGap <= hitRange &&
            verticalGap <= verticalHitRange)
        {
            PlayerHealth health =
                target.GetComponent<PlayerHealth>();

            if (health != null && !health.isDead)
            {
                health.TakeDamage(damage, transform.position);

                Debug.Log(
                    "BOSS DASH SLASH HIT! " +
                    target.name +
                    " TAKES " + damage + " DAMAGE!"
                );
            }
        }
        else
        {
            Debug.Log("BOSS DASH SLASH MISSED!");
        }

        Debug.Log("BOSS DASH END!");
    }

    private void StopHorizontalMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }
    }

    private bool IsTargetAlive(GameObject target)
    {
        if (target == null || !target.activeInHierarchy)
            return false;

        PlayerHealth health = target.GetComponent<PlayerHealth>();

        return health != null && !health.isDead;
    }
}

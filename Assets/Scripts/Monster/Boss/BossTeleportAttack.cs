
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossTeleportAttack : MonoBehaviour
{
    [Header("Teleport Animation")]
    public float teleportAnimationDuration = 0.35f;

    [Header("Teleport Settings")]
    public float disappearDuration = 0.5f;
    public float appearDelay = 0.5f;

    [Header("AoE Attack Settings")]
    public float attackRadius = 3f;
    public int damage = 40;
    public float attackWindup = 0.8f;

    [Header("AoE Position")]
    public float aoeGroundOffset = 0.5f;

    [Header("Animation")]
    public Animator animator;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D bossCollider;

    private bool cancelRequested = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        bossCollider = GetComponent<Collider2D>();

        if (bossCollider == null)
            bossCollider = GetComponentInChildren<Collider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void CancelTeleport()
    {
        cancelRequested = true;

        RestoreSprite();
    }

    public IEnumerator PerformTeleport(GameObject player1, GameObject player2)
    {
        cancelRequested = false;

        bool p1Alive = IsPlayerAlive(player1);
        bool p2Alive = IsPlayerAlive(player2);

        if (!p1Alive && !p2Alive)
            yield break;

        Vector2 targetPosition;

        if (p1Alive && p2Alive)
        {
            targetPosition =
                ((Vector2)player1.transform.position +
                (Vector2)player2.transform.position) / 2f;
        }
        else if (p1Alive)
        {
            targetPosition = player1.transform.position;
        }
        else
        {
            targetPosition = player2.transform.position;
        }

        targetPosition.y = transform.position.y;

        Debug.Log("BOSS TELEPORT START!");

        // 1. เล่น Animation Teleport ก่อน
        if (animator != null)
            animator.SetTrigger("Teleport");

        yield return new WaitForSeconds(teleportAnimationDuration);

        if (cancelRequested)
        {
            RestoreSprite();
            yield break;
        }

        // 2. ซ่อน Sprite
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSeconds(disappearDuration);

        if (cancelRequested)
        {
            RestoreSprite();
            yield break;
        }

        // 3. ย้ายตำแหน่งบอส
        if (rb != null)
        {
            rb.position = targetPosition;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            transform.position = targetPosition;
        }

        yield return new WaitForSeconds(appearDelay);

        if (cancelRequested)
        {
            RestoreSprite();
            yield break;
        }

        // 4. แสดง Sprite กลับมา
        RestoreSprite();

        Debug.Log("BOSS TELEPORTED!");

        // 5. เล่น Animation AttackCircle
        if (animator != null)
            animator.SetTrigger("AttackCircle");

        // 6. รอจังหวะโจมตี
        yield return new WaitForSeconds(attackWindup);

        if (cancelRequested)
            yield break;

        // 7. สร้างความเสียหายรอบตัว
        PerformAoEAttack();

        Debug.Log("BOSS TELEPORT ATTACK END!");
    }

    private void PerformAoEAttack()
    {
        Vector2 aoeCenter = GetAoECenter();

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            aoeCenter,
            attackRadius
        );

        HashSet<PlayerHealth> damagedPlayers =
            new HashSet<PlayerHealth>();

        foreach (Collider2D hit in hits)
        {
            PlayerHealth health =
                hit.GetComponent<PlayerHealth>();

            if (health == null)
                health = hit.GetComponentInParent<PlayerHealth>();

            if (health == null || health.isDead)
                continue;

            if (!damagedPlayers.Add(health))
                continue;

            health.TakeDamage(damage, transform.position);

            Debug.Log(
                "BOSS AOE HIT! " +
                health.gameObject.name +
                " TAKES " + damage + " DAMAGE!"
            );
        }
    }

    private Vector2 GetAoECenter()
    {
        if (bossCollider != null)
        {
            Bounds bounds = bossCollider.bounds;

            return new Vector2(
                bounds.center.x,
                bounds.min.y + aoeGroundOffset
            );
        }

        return (Vector2)transform.position +
            Vector2.up * aoeGroundOffset;
    }

    private void RestoreSprite()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }

    private bool IsPlayerAlive(GameObject player)
    {
        if (player == null || !player.activeInHierarchy)
            return false;

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        return health != null && !health.isDead;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 center;

        Collider2D col = GetComponent<Collider2D>();

        if (col == null)
            col = GetComponentInChildren<Collider2D>();

        if (col != null)
        {
            Bounds bounds = col.bounds;

            center = new Vector2(
                bounds.center.x,
                bounds.min.y + aoeGroundOffset
            );
        }
        else
        {
            center = (Vector2)transform.position +
                Vector2.up * aoeGroundOffset;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}

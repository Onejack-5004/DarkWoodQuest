
using UnityEngine;
using System.Collections;

public class MonsterFollow : MonoBehaviour
{
    [Header("Monster Data")]
    public MonsterData monsterData;

    [Header("Player References")]
    public Transform player1;
    public Transform player2;

    [Header("Animation")]
    public Animator animator;

    [Header("Attack Timing")]
    public float attackAnimationDelay = 0.3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D monsterCollider;

    private Collider2D player1Collider;
    private Collider2D player2Collider;

    private PlayerHealth player1Health;
    private PlayerHealth player2Health;

    private Transform targetPlayer;
    private PlayerHealth targetHealth;

    private float moveSpeed;
    private int attackDamage;
    private float attackRange;
    private float attackCooldown;

    private bool isAttacking = false;
    private bool isHitstunned = false;
    private bool isDead = false;

    private Coroutine attackCoroutine;
    private Coroutine hitstunCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        monsterCollider = GetComponent<Collider2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (monsterData == null)
        {
            Debug.LogError(
                "MonsterData is missing on " + gameObject.name
            );

            enabled = false;
            return;
        }

        moveSpeed = monsterData.moveSpeed;
        attackDamage = monsterData.attackDamage;
        attackRange = monsterData.attackRange;
        attackCooldown = monsterData.attackCooldown;

        MonsterHealth health = GetComponent<MonsterHealth>();

        if (health != null)
        {
            health.Initialize(monsterData.maxHealth);
        }

        SetupPlayers();
        UpdateAnimation(false);
    }

    private void SetupPlayers()
    {
        if (player1 != null)
        {
            player1Health = player1.GetComponent<PlayerHealth>();
            player1Collider = player1.GetComponent<Collider2D>();
        }

        if (player2 != null)
        {
            player2Health = player2.GetComponent<PlayerHealth>();
            player2Collider = player2.GetComponent<Collider2D>();
        }
    }

    private void Update()
    {
        if (isDead || isAttacking || isHitstunned)
            return;

        FindClosestPlayer();

        if (targetPlayer == null)
        {
            StopMovement();
            UpdateAnimation(false);
            return;
        }

        UpdateFacingDirection();

        float distance = GetDistanceToPlayer(targetPlayer);

        if (distance <= attackRange)
        {
            StopMovement();
            UpdateAnimation(false);

            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isAttacking || isHitstunned || rb == null)
            return;

        if (targetPlayer == null)
        {
            StopMovement();
            UpdateAnimation(false);
            return;
        }

        float distance = GetDistanceToPlayer(targetPlayer);

        if (distance > attackRange)
        {
            Vector2 direction = (
                (Vector2)targetPlayer.position - rb.position
            ).normalized;

            rb.linearVelocity = new Vector2(
                direction.x * moveSpeed,
                rb.linearVelocity.y
            );

            UpdateAnimation(true);
        }
        else
        {
            StopMovement();
            UpdateAnimation(false);
        }
    }

    private void FindClosestPlayer()
    {
        targetPlayer = null;
        targetHealth = null;

        float closestDistance = Mathf.Infinity;

        CheckPlayer(
            player1,
            player1Health,
            ref closestDistance
        );

        CheckPlayer(
            player2,
            player2Health,
            ref closestDistance
        );
    }

    private void CheckPlayer(
        Transform player,
        PlayerHealth health,
        ref float closestDistance
    )
    {
        if (player == null || health == null || health.isDead)
            return;

        float distance = GetDistanceToPlayer(player);

        if (distance < closestDistance)
        {
            closestDistance = distance;
            targetPlayer = player;
            targetHealth = health;
        }
    }

    private float GetDistanceToPlayer(Transform player)
    {
        Collider2D playerCollider = null;

        if (player == player1)
            playerCollider = player1Collider;
        else if (player == player2)
            playerCollider = player2Collider;

        if (monsterCollider != null && playerCollider != null)
        {
            Vector2 monsterPoint = monsterCollider.ClosestPoint(
                playerCollider.bounds.center
            );

            Vector2 playerPoint = playerCollider.ClosestPoint(
                monsterCollider.bounds.center
            );

            return Vector2.Distance(
                monsterPoint,
                playerPoint
            );
        }

        return Vector2.Distance(
            transform.position,
            player.position
        );
    }

    public Vector2 GetClosestPlayerPosition()
    {
        FindClosestPlayer();

        if (targetPlayer != null)
            return targetPlayer.position;

        return transform.position + Vector3.left;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        StopMovement();
        UpdateAnimation(false);

        // เล่น Attack Animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // รอจนถึงจังหวะที่กำหนดก่อนทำ Damage
        yield return new WaitForSeconds(
            attackAnimationDelay
        );

        if (isDead || isHitstunned)
            yield break;

        FindClosestPlayer();

        if (targetPlayer != null &&
            targetHealth != null &&
            !targetHealth.isDead)
        {
            float distance = GetDistanceToPlayer(targetPlayer);

            if (distance <= attackRange)
            {
                targetHealth.TakeDamage(
                    attackDamage,
                    transform.position
                );

                Debug.Log(
                    gameObject.name +
                    " attacked " +
                    targetPlayer.name +
                    " for " +
                    attackDamage +
                    " damage!"
                );
            }
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        attackCoroutine = null;
    }

    public void ApplyHitstun(
        float duration,
        Vector2 sourcePosition,
        float force
    )
    {
        if (isDead) return;

        // ยกเลิก Attack ที่กำลังทำอยู่
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;

        // ยกเลิก Hitstun เดิมหากโดนตีซ้ำ
        if (hitstunCoroutine != null)
        {
            StopCoroutine(hitstunCoroutine);
        }

        hitstunCoroutine = StartCoroutine(
            HitstunRoutine(duration, sourcePosition, force)
        );
    }

    private IEnumerator HitstunRoutine(
        float duration,
        Vector2 sourcePosition,
        float force
    )
    {
        isHitstunned = true;

        StopMovement();
        UpdateAnimation(false);

        // เล่น Hit Animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        float direction =
            transform.position.x >= sourcePosition.x ? 1f : -1f;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                direction * force,
                rb.linearVelocity.y
            );
        }

        yield return new WaitForSeconds(duration);

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }

        isHitstunned = false;
        hitstunCoroutine = null;
    }

    private void UpdateFacingDirection()
    {
        if (spriteRenderer == null || targetPlayer == null)
            return;

        spriteRenderer.flipX =
            targetPlayer.position.x < transform.position.x;
    }

    private void UpdateAnimation(bool isWalking)
    {
        if (animator == null) return;

        animator.SetBool("IsWalking", isWalking);
    }

    private void StopMovement()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(
            0,
            rb.linearVelocity.y
        );
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (hitstunCoroutine != null)
        {
            StopCoroutine(hitstunCoroutine);
            hitstunCoroutine = null;
        }

        isAttacking = false;
        isHitstunned = false;

        StopMovement();

        // สั่ง Death Animation ก่อนปิด AI
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Die");
        }

        // ปิดเฉพาะ AI ไม่ได้ปิด Animator
        enabled = false;
    }
}

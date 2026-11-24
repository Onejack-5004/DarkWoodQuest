
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerName = "Player 1";

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public int maxJumps = 2;

    [Header("Controls")]
    public Key leftKey = Key.A;
    public Key rightKey = Key.B;
    public Key jumpKey = Key.W;
    public Key attackKey = Key.J;
    public Key ultimateKey = Key.U;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Hitstun")]
    public float hitstunDuration = 0.3f;
    public float knockbackForce = 4f;

    [Header("Animation")]
    public Animator animator;

    [Header("Player State")]
    public bool canControl = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerHealth playerHealth;
    private Keyboard keyboard;

    private float moveInput;
    private bool isGrounded;
    private bool jumpRequested;
    private int jumpCount = 0;

    private bool isAutoWalking = false;
    private float autoWalkDirection = 1f;

    private bool isHitstunned = false;
    private Coroutine hitstunCoroutine;

    private bool isUltimateLocked = false;
    private bool isAttacking = false;
    private bool isDead = false;

    public bool IsHitstunned => isHitstunned;
    public bool IsAutoWalking => isAutoWalking;
    public bool IsUltimateLocked => isUltimateLocked;
    public bool IsGrounded => isGrounded;
    public bool IsAttacking => isAttacking;
    public bool IsDead => isDead;

    public Action OnAttackPressed;
    public Action OnUltimatePressed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        UpdateGrounded();
        UpdateAnimation(false);
    }

    private void Update()
    {
        UpdateGrounded();

        if (IsPlayerDead())
        {
            moveInput = 0;
            jumpRequested = false;
            isAutoWalking = false;
            UpdateAnimation(false);
            return;
        }

        if (isHitstunned)
        {
            moveInput = 0;
            jumpRequested = false;
            UpdateAnimation(false);
            return;
        }

        if (isUltimateLocked)
        {
            moveInput = 0;
            jumpRequested = false;
            UpdateAnimation(false);
            return;
        }

        // ล็อกการเดินและกระโดดระหว่าง Attack
        if (isAttacking)
        {
            moveInput = 0;
            jumpRequested = false;
            UpdateAnimation(false);
            return;
        }

        if (isAutoWalking)
        {
            moveInput = autoWalkDirection;
            jumpRequested = false;

            UpdateFacingDirection();
            UpdateAnimation(true);
            return;
        }

        keyboard = Keyboard.current;

        if (keyboard == null || !canControl)
        {
            moveInput = 0;
            jumpRequested = false;
            UpdateAnimation(false);
            return;
        }

        if (isGrounded)
        {
            jumpCount = 0;
        }

        moveInput = 0;

        if (keyboard[leftKey].isPressed)
        {
            moveInput = -1;
        }

        if (keyboard[rightKey].isPressed)
        {
            moveInput = 1;
        }

        UpdateFacingDirection();
        UpdateAnimation(moveInput != 0);

        // Jump
        if (keyboard[jumpKey].wasPressedThisFrame)
        {
            if (jumpCount < maxJumps)
            {
                jumpRequested = true;
                jumpCount++;
            }
        }

        // Attack
        if (keyboard[attackKey].wasPressedThisFrame)
        {
            OnAttackPressed?.Invoke();
        }

        // Ultimate
        if (keyboard[ultimateKey].wasPressedThisFrame)
        {
            OnUltimatePressed?.Invoke();
        }
    }

    private bool IsPlayerDead()
    {
        return isDead || (playerHealth != null && playerHealth.isDead);
    }

    private void UpdateGrounded()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        if (IsPlayerDead())
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
            return;
        }

        if (isHitstunned) return;

        // หยุดแนวนอนระหว่าง Ultimate และ Attack
        if (isUltimateLocked || isAttacking)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
            return;
        }

        if (!canControl && !isAutoWalking)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
            return;
        }

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            jumpRequested = false;
        }
    }

    private void UpdateFacingDirection()
    {
        if (spriteRenderer == null) return;

        if (moveInput > 0)
            spriteRenderer.flipX = false;
        else if (moveInput < 0)
            spriteRenderer.flipX = true;
    }

    private void UpdateAnimation(bool isWalking)
    {
        if (animator == null) return;

        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsGrounded", isGrounded);
    }

    // เริ่มโจมตีได้เฉพาะตอนอยู่บนพื้นและควบคุมตัวละครได้
    public bool TryStartAttackAnimation()
    {
        if (IsPlayerDead() ||
            !canControl ||
            !isGrounded ||
            isHitstunned ||
            isAutoWalking ||
            isUltimateLocked ||
            isAttacking)
        {
            return false;
        }

        isAttacking = true;

        moveInput = 0;
        jumpRequested = false;

        UpdateAnimation(false);

        if (animator != null)
        {
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Attack");
        }

        return true;
    }

    // เรียกจาก Animation Event ที่เฟรมสุดท้ายของ Attack
    public void EndAttackAnimation()
    {
        if (isDead) return;

        isAttacking = false;
    }

    public void PlayUltimateAnimation()
    {
        if (animator == null || IsPlayerDead()) return;

        animator.SetTrigger("Ultimate");
    }

    public void PlayHitAnimation()
    {
        if (animator == null || IsPlayerDead()) return;

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Hit");
    }

    public void ApplyHitstun(
        float duration,
        Vector2 sourcePosition,
        float force
    )
    {
        if (IsPlayerDead()) return;

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
        isAttacking = false;

        moveInput = 0;
        jumpRequested = false;

        UpdateAnimation(false);
        PlayHitAnimation();

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

    public void StartAutoWalk(float direction = 1f)
    {
        if (IsPlayerDead()) return;

        isAutoWalking = true;
        autoWalkDirection = Mathf.Sign(direction);

        if (autoWalkDirection == 0)
            autoWalkDirection = 1f;

        canControl = false;

        Debug.Log(playerName + " Started Auto Walk!");
    }

    public void StopAutoWalk()
    {
        isAutoWalking = false;
        moveInput = 0;

        if (!IsPlayerDead())
        {
            canControl = true;
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }

        Debug.Log(playerName + " Stopped Auto Walk!");
    }

    public void SetControlEnabled(bool enabled)
    {
        if (IsPlayerDead()) return;

        canControl = enabled;

        if (!enabled)
        {
            moveInput = 0;
            jumpRequested = false;
        }
    }

    public void SetUltimateLocked(bool locked)
    {
        if (IsPlayerDead() && !locked) return;

        isUltimateLocked = locked;

        if (locked)
        {
            moveInput = 0;
            jumpRequested = false;

            if (rb != null)
            {
                rb.linearVelocity = new Vector2(
                    0,
                    rb.linearVelocity.y
                );
            }

            UpdateAnimation(false);
        }
    }

    // เรียกเมื่อ HP เหลือ 0
    public void HandleDeath()
    {
        if (isDead) return;

        isDead = true;

        canControl = false;
        isAutoWalking = false;
        isAttacking = false;
        isHitstunned = false;
        isUltimateLocked = false;

        moveInput = 0;
        jumpRequested = false;

        if (hitstunCoroutine != null)
        {
            StopCoroutine(hitstunCoroutine);
            hitstunCoroutine = null;
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Ultimate");
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Die");
        }

        Debug.Log(playerName + " Death Animation!");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }
    }
}

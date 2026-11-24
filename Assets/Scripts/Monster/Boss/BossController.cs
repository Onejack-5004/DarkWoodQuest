
using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public enum BossAttackType
    {
        None,
        Melee,
        Dash,
        Teleport
    }

    [Header("Player References")]
    public GameObject player1;
    public GameObject player2;

    [Header("Boss Components")]
    public BossHealth bossHealth;
    public BossMeleeAttack bossMeleeAttack;
    public BossDashAttack bossDashAttack;
    public BossTeleportAttack teleportAttack;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer bossSprite;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;

    [Header("Melee Settings")]
    public float meleeWindup = 0.35f;
    public float meleeRecovery = 0.65f;

    [Header("Ultimate Settings")]
    public Transform ultimatePoint;
    public float ultimateWindup = 2f;

    [Header("Target Settings")]
    public GameObject currentTarget;

    [Header("Boss State")]
    public BossAttackType lastAttack = BossAttackType.None;
    public bool isAttacking = false;
    public bool isUsingUltimate = false;

    private bool battleStarted = false;

    private Coroutine battleCoroutine;
    private Coroutine ultimateCoroutine;

    private GameObject lastDashTarget;

    private Rigidbody2D bossRb;
    private float originalGravityScale;

    private void Awake()
    {
        if (bossHealth == null)
            bossHealth = GetComponent<BossHealth>();

        if (bossMeleeAttack == null)
            bossMeleeAttack = GetComponent<BossMeleeAttack>();

        if (bossDashAttack == null)
            bossDashAttack = GetComponent<BossDashAttack>();

        if (teleportAttack == null)
            teleportAttack = GetComponent<BossTeleportAttack>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (bossSprite == null)
            bossSprite = GetComponentInChildren<SpriteRenderer>();

        bossRb = GetComponent<Rigidbody2D>();

        if (bossRb != null)
            originalGravityScale = bossRb.gravityScale;
    }

    private void OnEnable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnUltimateThresholdReached += HandleUltimateThreshold;
            bossHealth.OnBossDied += HandleBossDied;
        }
    }

    private void OnDisable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnUltimateThresholdReached -= HandleUltimateThreshold;
            bossHealth.OnBossDied -= HandleBossDied;
        }
    }

    private void Start()
    {
        if (bossHealth == null ||
            bossMeleeAttack == null ||
            bossDashAttack == null ||
            teleportAttack == null)
        {
            Debug.LogError("Boss Components are missing!");
            return;
        }

        if (animator == null)
            Debug.LogWarning("Boss Animator is missing!");

        bossHealth.SetInvulnerable(true);

        Debug.Log("BOSS WAITING FOR INTRO DIALOGUE!");
    }

    // =====================================
    // ANIMATION
    // =====================================

    private void PlayAnimation(string triggerName)
    {
        if (animator == null)
            return;

        animator.SetTrigger(triggerName);
    }

    private void ResetAnimationTriggers()
    {
        if (animator == null)
            return;

        animator.ResetTrigger("Melee");
        animator.ResetTrigger("Dash");
        animator.ResetTrigger("Teleport");
        animator.ResetTrigger("AttackCircle");
        animator.ResetTrigger("Ultimate");
        animator.ResetTrigger("Death");
    }

    // =====================================
    // BOSS BATTLE
    // =====================================

    public void StartBattle()
    {
        if (battleStarted ||
            bossHealth == null ||
            bossHealth.isDead ||
            isUsingUltimate)
        {
            return;
        }

        battleStarted = true;

        bossHealth.SetInvulnerable(false);

        Debug.Log("BOSS BATTLE STARTED!");

        battleCoroutine = StartCoroutine(BossBattleRoutine());
    }

    private IEnumerator BossBattleRoutine()
    {
        while (battleStarted)
        {
            if (bossHealth.isDead || isUsingUltimate)
                break;

            currentTarget = GetTarget();

            if (currentTarget == null)
            {
                battleStarted = false;
                Debug.Log("BOSS HAS NO LIVING TARGET!");
                break;
            }

            BossAttackType nextAttack = ChooseNextAttack();

            if (nextAttack == BossAttackType.None)
            {
                yield return null;
                continue;
            }

            yield return StartCoroutine(ExecuteAttack(nextAttack));

            if (battleStarted)
                yield return new WaitForSeconds(attackCooldown);
        }

        battleStarted = false;
        battleCoroutine = null;
    }

    // =====================================
    // TARGET SELECTION
    // =====================================

    private GameObject GetTarget()
    {
        bool p1Alive = IsPlayerAlive(player1);
        bool p2Alive = IsPlayerAlive(player2);

        if (!p1Alive && !p2Alive)
            return null;

        if (p1Alive && !p2Alive)
            return player1;

        if (!p1Alive && p2Alive)
            return player2;

        float distance1 = Vector2.Distance(
            transform.position,
            player1.transform.position
        );

        float distance2 = Vector2.Distance(
            transform.position,
            player2.transform.position
        );

        return distance1 <= distance2 ? player1 : player2;
    }

    private GameObject GetDashTarget()
    {
        bool p1Alive = IsPlayerAlive(player1);
        bool p2Alive = IsPlayerAlive(player2);

        if (!p1Alive && !p2Alive)
            return null;

        if (p1Alive && !p2Alive)
            return player1;

        if (!p1Alive && p2Alive)
            return player2;

        if (lastDashTarget == player1)
            return player2;

        if (lastDashTarget == player2)
            return player1;

        return GetTarget();
    }

    private bool IsPlayerAlive(GameObject player)
    {
        if (player == null || !player.activeInHierarchy)
            return false;

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        return health != null && !health.isDead;
    }

    // =====================================
    // TELEPORT ATTACK CONDITION
    // =====================================

    private bool CanUseTeleportAttack()
    {
        bool p1Alive = IsPlayerAlive(player1);
        bool p2Alive = IsPlayerAlive(player2);

        if (!p1Alive && !p2Alive)
            return false;

        if (p1Alive && !p2Alive)
            return true;

        if (!p1Alive && p2Alive)
            return true;

        float playerDistance = Vector2.Distance(
            player1.transform.position,
            player2.transform.position
        );

        return playerDistance <= teleportAttack.attackRadius;
    }

    // =====================================
    // ATTACK SELECTION
    // =====================================

    private BossAttackType ChooseNextAttack()
    {
        bool playerInMeleeRange = false;

        float meleeRange = bossMeleeAttack.attackRange;

        if (IsPlayerAlive(player1))
        {
            float distance = Vector2.Distance(
                transform.position,
                player1.transform.position
            );

            if (distance <= meleeRange)
                playerInMeleeRange = true;
        }

        if (IsPlayerAlive(player2))
        {
            float distance = Vector2.Distance(
                transform.position,
                player2.transform.position
            );

            if (distance <= meleeRange)
                playerInMeleeRange = true;
        }

        BossAttackType[] possibleAttacks =
            new BossAttackType[3];

        int count = 0;

        if (playerInMeleeRange &&
            lastAttack != BossAttackType.Melee)
        {
            possibleAttacks[count++] = BossAttackType.Melee;
        }

        if (lastAttack != BossAttackType.Dash)
        {
            possibleAttacks[count++] = BossAttackType.Dash;
        }

        if (CanUseTeleportAttack() &&
            lastAttack != BossAttackType.Teleport)
        {
            possibleAttacks[count++] = BossAttackType.Teleport;
        }

        if (count == 0)
            return BossAttackType.None;

        BossAttackType selectedAttack =
            possibleAttacks[Random.Range(0, count)];

        lastAttack = selectedAttack;

        return selectedAttack;
    }

    // =====================================
    // EXECUTE ATTACK
    // =====================================

    private IEnumerator ExecuteAttack(BossAttackType attack)
    {
        isAttacking = true;

        switch (attack)
        {
            case BossAttackType.Melee:

                currentTarget = GetTarget();

                Debug.Log("BOSS: MELEE ATTACK!");

                PlayAnimation("Melee");

                yield return new WaitForSeconds(meleeWindup);

                if (!bossHealth.isDead &&
                    !isUsingUltimate &&
                    currentTarget != null &&
                    IsPlayerAlive(currentTarget))
                {
                    bossMeleeAttack.PerformAttack(currentTarget);
                }

                yield return new WaitForSeconds(meleeRecovery);

                break;

            case BossAttackType.Dash:

                currentTarget = GetDashTarget();

                if (currentTarget != null)
                {
                    lastDashTarget = currentTarget;

                    Debug.Log(
                        "BOSS: DASH TARGET = " +
                        currentTarget.name
                    );

                    PlayAnimation("Dash");

                    yield return StartCoroutine(
                        bossDashAttack.PerformDash(currentTarget)
                    );
                }

                break;

            case BossAttackType.Teleport:

                yield return StartCoroutine(
                    teleportAttack.PerformTeleport(
                        player1,
                        player2
                    )
                );

                break;
        }

        isAttacking = false;
    }

    // =====================================
    // INTERRUPT ALL CURRENT ACTIONS
    // =====================================

    private void InterruptCurrentActions()
    {
        Debug.Log("BOSS ACTIONS INTERRUPTED!");

        if (battleCoroutine != null)
        {
            StopCoroutine(battleCoroutine);
            battleCoroutine = null;
        }

        battleStarted = false;
        isAttacking = false;

        if (bossDashAttack != null)
            bossDashAttack.CancelDash();

        if (teleportAttack != null)
            teleportAttack.CancelTeleport();

        StopBossMovement();
    }

    private void StopBossMovement()
    {
        if (bossRb == null)
            return;

        bossRb.linearVelocity = Vector2.zero;
        bossRb.angularVelocity = 0f;
        bossRb.gravityScale = 0f;
    }

    private void RestoreBossMovement()
    {
        if (bossRb == null)
            return;

        bossRb.linearVelocity = Vector2.zero;
        bossRb.angularVelocity = 0f;
        bossRb.gravityScale = originalGravityScale;
    }

    // =====================================
    // BOSS ULTIMATE
    // =====================================

    private void HandleUltimateThreshold()
    {
        if (isUsingUltimate || bossHealth.isDead)
            return;

        Debug.Log("BOSS ULTIMATE THRESHOLD REACHED!");

        InterruptCurrentActions();

        ultimateCoroutine = StartCoroutine(UltimateRoutine());
    }

    private void TeleportToUltimatePoint()
    {
        if (ultimatePoint == null)
        {
            Debug.LogWarning(
                "Boss Ultimate Point is missing! " +
                "Boss will use its current position."
            );

            return;
        }

        Vector2 targetPosition = ultimatePoint.position;

        if (bossRb != null)
            bossRb.position = targetPosition;
        else
            transform.position = targetPosition;

        Debug.Log("BOSS TELEPORTED TO ULTIMATE POINT!");
    }

    private IEnumerator UltimateRoutine()
    {
        isUsingUltimate = true;
        isAttacking = false;

        bossHealth.SetInvulnerable(true);

        StopBossMovement();

        Debug.Log("BOSS ULTIMATE ACTIVATED!");

        TeleportToUltimatePoint();

        PlayAnimation("Ultimate");

        yield return new WaitForSeconds(0.3f);

        if (bossHealth.isDead)
            yield break;

        Debug.Log("BOSS IS CHARGING ULTIMATE!");

        yield return new WaitForSeconds(ultimateWindup);

        if (bossHealth.isDead)
            yield break;

        Debug.Log("BOSS ULTIMATE RELEASED!");

        ApplyUltimateDamage(player1);
        ApplyUltimateDamage(player2);

        Debug.Log("BOSS ULTIMATE FINISHED!");

        RestoreBossMovement();

        bossHealth.SetInvulnerable(false);

        isUsingUltimate = false;
        ultimateCoroutine = null;

        StartBattle();

        Debug.Log("BOSS RETURNS TO NORMAL ATTACKS!");
    }

    private void ApplyUltimateDamage(GameObject player)
    {
        if (!IsPlayerAlive(player))
            return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        health.SetHealthToOne();
    }

    // =====================================
    // BOSS DEATH
    // =====================================

    private void HandleBossDied()
    {
        battleStarted = false;
        isAttacking = false;
        isUsingUltimate = false;

        StopAllCoroutines();

        if (bossDashAttack != null)
            bossDashAttack.CancelDash();

        if (teleportAttack != null)
            teleportAttack.CancelTeleport();

        StopBossMovement();

        battleCoroutine = null;
        ultimateCoroutine = null;

        bossHealth.SetInvulnerable(true);

        ResetAnimationTriggers();
        PlayAnimation("Death");

        Debug.Log("BOSS CONTROLLER STOPPED!");
    }
    private void Update()
    {
        if (bossHealth == null || bossHealth.isDead)
            return;

        if (bossSprite == null)
            return;

        GameObject target = GetTarget();

        if (target == null)
            return;

        // หันไปทางผู้เล่น
        bossSprite.flipX =
            target.transform.position.x < transform.position.x;
    }
}

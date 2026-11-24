
using UnityEngine;
using System.Collections.Generic;

public class MeleeAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public LayerMask monsterLayer;
    public LayerMask bossLayer;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack(int damage, float range)
    {
        float direction = 1f;

        if (spriteRenderer != null && spriteRenderer.flipX)
            direction = -1f;

        Vector2 attackPosition =
            (Vector2)transform.position +
            new Vector2(direction * range, 0f);

        // รวม Layer ของ Monster และ Boss
        LayerMask targetLayer = monsterLayer | bossLayer;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPosition,
            range,
            targetLayer
        );

        HashSet<MonsterHealth> damagedMonsters =
            new HashSet<MonsterHealth>();

        HashSet<BossHealth> damagedBosses =
            new HashSet<BossHealth>();

        foreach (Collider2D hit in hits)
        {
            MonsterHealth monster =
                hit.GetComponentInParent<MonsterHealth>();

            if (monster != null && damagedMonsters.Add(monster))
            {
                monster.TakeDamage(damage);
            }

            BossHealth boss =
                hit.GetComponentInParent<BossHealth>();

            if (boss != null && damagedBosses.Add(boss))
            {
                boss.TakeDamage(damage);
            }
        }

        Debug.Log(gameObject.name + " Melee Attack!");
    }

    private void OnDrawGizmosSelected()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float direction = 1f;

        if (sr != null && sr.flipX)
            direction = -1f;

        Vector2 attackPosition =
            (Vector2)transform.position +
            new Vector2(direction * 0.8f, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, 0.8f);
    }
}

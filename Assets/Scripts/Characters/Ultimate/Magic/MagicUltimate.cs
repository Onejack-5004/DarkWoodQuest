
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicUltimate : PlayerUltimateAbility
{
    [Header("Magic Ultimate")]
    public int damage = 100;

    public float attackRange = 2.5f;
    public float attackHeight = 2f;

    public float attackDuration = 0.35f;

    private SpriteRenderer spriteRenderer;

    public override UltimateType AbilityType => UltimateType.Magic;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override IEnumerator ExecuteUltimate()
    {
        Debug.Log("Magic Ultimate: Staff Smash!");

        // ตรวจสอบทิศทางที่ตัวละครหัน
        float direction = 1f;

        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            direction = -1f;
        }

        // กำหนดพื้นที่โจมตีด้านหน้า
        Vector2 attackCenter =
            (Vector2)transform.position +
            Vector2.right * direction * (attackRange / 2f);

        Vector2 attackSize = new Vector2(
            attackRange,
            attackHeight
        );

        // ตรวจจับศัตรูในพื้นที่
        Collider2D[] targets = Physics2D.OverlapBoxAll(
            attackCenter,
            attackSize,
            0f
        );

        HashSet<MonsterHealth> hitMonsters =
            new HashSet<MonsterHealth>();

        HashSet<BossHealth> hitBosses =
            new HashSet<BossHealth>();

        foreach (Collider2D target in targets)
        {
            if (target.GetComponentInParent<PlayerHealth>() != null)
                continue;

            MonsterHealth monster =
                target.GetComponentInParent<MonsterHealth>();

            if (monster != null)
            {
                if (hitMonsters.Add(monster))
                {
                    monster.TakeDamage(damage);
                    Debug.Log("Magic Ultimate hit Monster!");
                }

                continue;
            }

            BossHealth boss =
                target.GetComponentInParent<BossHealth>();

            if (boss != null)
            {
                if (hitBosses.Add(boss))
                {
                    boss.TakeDamage(damage);
                    Debug.Log("Magic Ultimate hit Boss!");
                }
            }
        }

        // รอให้ช่วงโจมตีจบก่อนปลดล็อกตัวละคร
        yield return new WaitForSeconds(attackDuration);
    }
}

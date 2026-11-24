
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FistUltimate : PlayerUltimateAbility
{
    [Header("Fist Ultimate")]
    public int damage = 120;

    public float attackRadius = 2.5f;

    public float attackDuration = 0.4f;

    public override UltimateType AbilityType => UltimateType.Fist;

    public override IEnumerator ExecuteUltimate()
    {
        Debug.Log("Fist Ultimate: POWER SHOCKWAVE!");

        // ตรวจจับศัตรูทั้งหมดรอบตัวละคร
        Collider2D[] targets = Physics2D.OverlapCircleAll(
            transform.position,
            attackRadius
        );

        // ป้องกันการโจมตีศัตรูตัวเดิมซ้ำ
        HashSet<MonsterHealth> hitMonsters =
            new HashSet<MonsterHealth>();

        HashSet<BossHealth> hitBosses =
            new HashSet<BossHealth>();

        foreach (Collider2D target in targets)
        {
            // ไม่โจมตีผู้เล่น
            if (target.GetComponentInParent<PlayerHealth>() != null)
                continue;

            // ตรวจสอบ Monster
            MonsterHealth monster =
                target.GetComponentInParent<MonsterHealth>();

            if (monster != null)
            {
                if (hitMonsters.Add(monster))
                {
                    monster.TakeDamage(damage);

                    Debug.Log("Fist Ultimate hit Monster!");
                }

                continue;
            }

            // ตรวจสอบ Boss
            BossHealth boss =
                target.GetComponentInParent<BossHealth>();

            if (boss != null)
            {
                if (hitBosses.Add(boss))
                {
                    boss.TakeDamage(damage);

                    Debug.Log("Fist Ultimate hit Boss!");
                }
            }
        }

        // รอให้ช่วงโจมตีจบ
        yield return new WaitForSeconds(attackDuration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRadius
        );
    }
}

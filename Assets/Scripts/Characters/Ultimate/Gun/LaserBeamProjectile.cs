
using UnityEngine;
using System.Collections.Generic;

public class LaserBeamProjectile : MonoBehaviour
{
    [Header("Target Layers")]
    public LayerMask monsterLayer;
    public LayerMask bossLayer;

    private int damage;
    private float lifetime;

    private bool initialized = false;

    private HashSet<MonsterHealth> hitMonsters =
        new HashSet<MonsterHealth>();

    private HashSet<BossHealth> hitBosses =
        new HashSet<BossHealth>();

    public void Initialize(
        float direction,
        int attackDamage,
        float beamLength,
        float beamThickness,
        float duration
    )
    {
        damage = attackDamage;
        lifetime = duration;

        // ปรับขนาดลำแสง
        transform.localScale = new Vector3(
            beamLength,
            beamThickness,
            1f
        );

        initialized = true;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized) return;

        // ไม่โจมตีผู้เล่น
        if (other.GetComponentInParent<PlayerHealth>() != null)
            return;

        // ตรวจสอบ Monster
        MonsterHealth monster =
            other.GetComponentInParent<MonsterHealth>();

        if (monster != null)
        {
            bool isMonster =
                ((1 << monster.gameObject.layer) & monsterLayer.value) != 0 ||
                ((1 << other.gameObject.layer) & monsterLayer.value) != 0;

            if (isMonster && hitMonsters.Add(monster))
            {
                monster.TakeDamage(damage);

                Debug.Log("Laser hit Monster!");
            }

            return;
        }

        // ตรวจสอบ Boss
        BossHealth boss =
            other.GetComponentInParent<BossHealth>();

        if (boss != null)
        {
            bool isBoss =
                ((1 << boss.gameObject.layer) & bossLayer.value) != 0 ||
                ((1 << other.gameObject.layer) & bossLayer.value) != 0;

            if (isBoss && hitBosses.Add(boss))
            {
                boss.TakeDamage(damage);

                Debug.Log("Laser hit Boss!");
            }
        }
    }
}

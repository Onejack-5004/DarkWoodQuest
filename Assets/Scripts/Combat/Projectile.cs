
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Explosion Visual")]
    public GameObject explosionPrefab;

    private Rigidbody2D rb;

    private WeaponData weaponData;
    private LayerMask targetLayer;

    private bool hasExploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(
    float direction,
    WeaponData data,
    LayerMask targetLayer)
    {
        weaponData = data;
        this.targetLayer = targetLayer;

        // กำหนดทิศทางการเคลื่อนที่
        rb.linearVelocity = new Vector2(
            direction * weaponData.projectileSpeed,
            0f
        );

        // พลิกภาพตามทิศทาง
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            sr.flipX = direction < 0;
        }

        Destroy(gameObject, weaponData.projectileLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;

        int otherLayer = 1 << other.gameObject.layer;

        // ระเบิดเมื่อชน Monster หรือ Boss
        if ((targetLayer.value & otherLayer) == 0)
            return;

        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        if (explosionPrefab != null)
        {
            Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        rb.linearVelocity = Vector2.zero;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            weaponData.explosionRadius,
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
                monster.TakeDamage(
                    weaponData.explosionDamage
                );
            }

            BossHealth boss =
                hit.GetComponentInParent<BossHealth>();

            if (boss != null && damagedBosses.Add(boss))
            {
                boss.TakeDamage(
                    weaponData.explosionDamage
                );
            }
        }

        Debug.Log("Fireball Explosion!");

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (weaponData == null) return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            weaponData.explosionRadius
        );
    }
}


using UnityEngine;
using System.Collections.Generic;

public class SwordWaveProjectile : MonoBehaviour
{
    [Header("Target Layers")]
    public LayerMask monsterLayer;
    public LayerMask bossLayer;

    private float direction;
    private float speed;
    private int damage;
    private float lifetime;

    private bool initialized = false;

    private SpriteRenderer spriteRenderer;

    private HashSet<MonsterHealth> hitMonsters =
        new HashSet<MonsterHealth>();

    private HashSet<BossHealth> hitBosses =
        new HashSet<BossHealth>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(
        float moveDirection,
        float moveSpeed,
        int attackDamage,
        float duration
    )
    {
        direction = Mathf.Sign(moveDirection);
        speed = moveSpeed;
        damage = attackDamage;
        lifetime = duration;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction < 0;
        }

        initialized = true;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!initialized) return;

        // เคลื่อนที่ไปข้างหน้า
        transform.position +=
            Vector3.right * direction * speed * Time.deltaTime;

      
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized) return;

        // ไม่โจมตีผู้เล่น
        if (other.GetComponentInParent<PlayerHealth>() != null)
            return;

        // ตรวจสอบ Monster
        if (((1 << other.gameObject.layer) & monsterLayer.value) != 0)
        {
            MonsterHealth monster =
                other.GetComponentInParent<MonsterHealth>();

            if (monster != null && hitMonsters.Add(monster))
            {
                monster.TakeDamage(damage);
                Debug.Log("Sword Projectile hit Monster!");
            }

            return;
        }

        // ตรวจสอบ Boss
        if (((1 << other.gameObject.layer) & bossLayer.value) != 0)
        {
            BossHealth boss =
                other.GetComponentInParent<BossHealth>();

            if (boss != null && hitBosses.Add(boss))
            {
                boss.TakeDamage(damage);
                Debug.Log("Sword Projectile hit Boss!");
            }
        }
    }
}

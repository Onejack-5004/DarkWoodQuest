
using UnityEngine;
using System;

public class MonsterHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Hitstun Settings")]
    public float hitstunDuration = 0.25f;
    public float knockbackForce = 3f;

    [Header("Monster State")]
    public bool isDead = false;

    private MonsterFollow monsterFollow;

    public event Action<MonsterHealth> OnDied;

    private void Awake()
    {
        monsterFollow = GetComponent<MonsterFollow>();
        currentHealth = maxHealth;
    }

    public void Initialize(int health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        Vector2 sourcePosition = transform.position;

        if (monsterFollow != null)
        {
            sourcePosition =
                monsterFollow.GetClosestPlayerPosition();
        }

        TakeDamage(damage, sourcePosition);
    }

    public void TakeDamage(
        int damage,
        Vector2 sourcePosition
    )
    {
        if (isDead || damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log(
            gameObject.name +
            " HP: " + currentHealth +
            "/" + maxHealth
        );

        // HP หมด
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // ยังไม่ตาย ให้เล่น Hit Animation ผ่านระบบ Hitstun
        if (monsterFollow != null)
        {
            monsterFollow.ApplyHitstun(
                hitstunDuration,
                sourcePosition,
                knockbackForce
            );
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log(gameObject.name + " Died!");

        // หยุด AI และสั่งเล่น Death Animation
        if (monsterFollow != null)
        {
            monsterFollow.Die();
        }

        // แจ้งระบบ Wave Manager ว่ามอนสเตอร์ตายแล้ว
        OnDied?.Invoke(this);
    }

    // เรียกจาก Animation Event ในเฟรมสุดท้ายของ Death
    public void DestroyMonster()
    {
        if (!isDead) return;

        Destroy(gameObject);
    }
}

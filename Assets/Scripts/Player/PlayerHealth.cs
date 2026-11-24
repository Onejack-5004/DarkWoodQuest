
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;

    [Header("Current Health")]
    public int currentHealth;

    [Header("Hitstun Settings")]
    public float hitstunDuration = 0.3f;
    public float knockbackForce = 4f;

    [Header("Player State")]
    public bool isDead = false;

    private PlayerController playerController;
    private Rigidbody2D rb;

    public Action<int, int> OnHealthChanged;
    public Action OnPlayerDied;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(
            damage,
            (Vector2)transform.position + Vector2.left
        );
    }

    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (isDead || damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log(
            gameObject.name +
            " HP: " + currentHealth
        );

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (playerController != null)
        {
            playerController.ApplyHitstun(
                hitstunDuration,
                sourcePosition,
                knockbackForce
            );
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log(
            gameObject.name +
            " Healed! HP: " + currentHealth
        );

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }

        if (playerController != null)
        {
            playerController.HandleDeath();
        }

        Debug.Log(gameObject.name + " Died!");

        OnPlayerDied?.Invoke();
    }

    public void SetHealthToOne()
    {
        if (isDead) return;

        currentHealth = 1;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log(
            gameObject.name +
            " HP SET TO 1 BY BOSS ULTIMATE!"
        );
    }
}


using UnityEngine;
using System;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health Settings")]
    public int maxHealth = 1000;

    [Header("Current Health")]
    public int currentHealth;

    [Header("Boss State")]
    public bool isDead = false;
    public bool isInvulnerable = false;

    [Header("Ultimate Settings")]
    [Range(0f, 1f)]
    public float ultimateHealthThreshold = 0.1f;

    [Header("Debug")]
    public bool ultimateTriggered = false;

    public event Action<int, int> OnHealthChanged;
    public event Action OnUltimateThresholdReached;
    public event Action OnBossDied;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentHealth = maxHealth;
        isDead = false;
        isInvulnerable = false;
        ultimateTriggered = false;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log(
            "BOSS INITIALIZED! HP: " +
            currentHealth + "/" + maxHealth
        );
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable || damage <= 0)
            return;

        int newHealth = Mathf.Max(
            currentHealth - damage,
            0
        );

        // ยังไม่ได้ใช้ Ultimate และกำลังจะเลือดต่ำกว่าเกณฑ์
        if (!ultimateTriggered)
        {
            int thresholdHealth = Mathf.CeilToInt(
                maxHealth * ultimateHealthThreshold
            );

            if (currentHealth > thresholdHealth &&
                newHealth <= thresholdHealth)
            {
                // ล็อกเลือดไว้ที่เกณฑ์ Ultimate
                currentHealth = thresholdHealth;

                Debug.Log(
                    "BOSS HP LOCKED AT ULTIMATE THRESHOLD!"
                );

                OnHealthChanged?.Invoke(
                    currentHealth,
                    maxHealth
                );

                CheckUltimateThreshold();

                return;
            }
        }

        currentHealth = newHealth;

        Debug.Log(
            "BOSS HIT! HP: " +
            currentHealth + "/" + maxHealth
        );

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void CheckUltimateThreshold()
    {
        if (ultimateTriggered)
            return;

        float healthPercent =
            (float)currentHealth / maxHealth;

        if (healthPercent <= ultimateHealthThreshold)
        {
            ultimateTriggered = true;

            Debug.Log("BOSS ULTIMATE READY!");

            OnUltimateThresholdReached?.Invoke();
        }
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;

        Debug.Log(
            "BOSS INVULNERABLE: " + isInvulnerable
        );
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Min(
            currentHealth,
            maxHealth
        );

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isInvulnerable = true;

        Debug.Log("BOSS DEFEATED!");

        OnBossDied?.Invoke();
    }
}

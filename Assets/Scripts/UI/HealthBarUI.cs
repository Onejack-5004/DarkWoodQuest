
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Target Player")]
    public PlayerHealth targetHealth;

    [Header("Health Bar")]
    public Image fillImage;

    [Header("Player Settings")]
    public bool isPlayer2 = false;

    private void OnEnable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void Start()
    {
        // ซ่อน HP Bar ของ Player 2 ถ้าเล่น Single Play
        if (isPlayer2 && !GameSession.isDuoPlay)
        {
            gameObject.SetActive(false);
            return;
        }

        // แสดง HP ปัจจุบันทันที
        if (targetHealth != null)
        {
            UpdateHealthBar(
                targetHealth.currentHealth,
                targetHealth.maxHealth
            );
        }
    }

    private void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (fillImage == null || maxHealth <= 0)
            return;

        float healthPercent =
            (float)currentHealth / maxHealth;

        fillImage.fillAmount = Mathf.Clamp01(healthPercent);
    }
}

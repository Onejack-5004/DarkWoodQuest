
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Boss Reference")]
    public BossHealth bossHealth;

    [Header("UI References")]
    public GameObject bossHPPanel;
    public Image hpFill;

    private void OnEnable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += UpdateHealthBar;
            bossHealth.OnBossDied += HideBossHP;
        }
    }

    private void OnDisable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateHealthBar;
            bossHealth.OnBossDied -= HideBossHP;
        }
    }

    private void Start()
    {
        // ซ่อน HP Bar ตอนเริ่มเกม
        HideBossHP();
    }

    public void ShowBossHP()
    {
        if (bossHealth == null || hpFill == null)
        {
            Debug.LogError("BossHealthUI references are missing!");
            return;
        }

        if (bossHealth.isDead)
            return;

        bossHPPanel.SetActive(true);

        // อัปเดตเลือดปัจจุบันทันที
        UpdateHealthBar(
            bossHealth.currentHealth,
            bossHealth.maxHealth
        );

        Debug.Log("BOSS HP BAR SHOWN!");
    }

    public void HideBossHP()
    {
        if (bossHPPanel != null)
        {
            bossHPPanel.SetActive(false);
        }
    }

    private void UpdateHealthBar(int currentHP, int maxHP)
    {
        if (hpFill == null || maxHP <= 0)
            return;

        float healthPercent =
            (float)currentHP / maxHP;

        hpFill.fillAmount = Mathf.Clamp01(healthPercent);
    }
}

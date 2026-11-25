
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Boss Reference")]
    public BossHealth bossHealth;

    [Header("Player References")]
    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    [Header("UI References")]
    public GameObject bossHPPanel;
    public Image hpFill;

    private void OnEnable()
    {
        // Subscribe Boss Events
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += UpdateHealthBar;
            bossHealth.OnBossDied += HideBossHP;
        }

        // Subscribe Player Events
        if (player1Health != null)
        {
            player1Health.OnPlayerDied += CheckPlayersAlive;
        }

        if (player2Health != null)
        {
            player2Health.OnPlayerDied += CheckPlayersAlive;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe Boss Events
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateHealthBar;
            bossHealth.OnBossDied -= HideBossHP;
        }

        // Unsubscribe Player Events
        if (player1Health != null)
        {
            player1Health.OnPlayerDied -= CheckPlayersAlive;
        }

        if (player2Health != null)
        {
            player2Health.OnPlayerDied -= CheckPlayersAlive;
        }
    }

    private void Start()
    {
        HideBossHP();
    }

    public void ShowBossHP()
    {
        if (bossHealth == null || hpFill == null || bossHPPanel == null)
        {
            Debug.LogError("BossHealthUI references are missing!");
            return;
        }

        if (bossHealth.isDead || !HasLivingPlayer())
        {
            HideBossHP();
            return;
        }

        bossHPPanel.SetActive(true);

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

    // ตรวจสอบว่ามีผู้เล่นคนไหนยังมีชีวิตอยู่หรือไม่
    private bool HasLivingPlayer()
    {
        bool player1Alive =
            player1Health != null && !player1Health.isDead;

        bool player2Alive =
            player2Health != null && !player2Health.isDead;

        if (GameSession.isDuoPlay)
        {
            return player1Alive || player2Alive;
        }

        return player1Alive;
    }

    // เรียกเมื่อผู้เล่นคนใดคนหนึ่งตาย
    private void CheckPlayersAlive()
    {
        if (!HasLivingPlayer())
        {
            HideBossHP();
            Debug.Log("ALL PLAYERS DEAD - BOSS HP BAR HIDDEN!");
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

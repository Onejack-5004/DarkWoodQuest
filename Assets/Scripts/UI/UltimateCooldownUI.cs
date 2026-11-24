using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateCooldownUI : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerUltimateController ultimateController;

    [Header("UI Elements")]
    public Image cooldownFillImage;     // ตั้ง Image Type = Filled (ถ้ามี)
    public TMP_Text cooldownText;        // ตัวเลขวินาที เช่น 11.5
    public GameObject cooldownOverlay;   // แผงสีทึบตอนติดคูลดาวน์ (ถ้ามี)

    [Header("Options")]
    public bool showDecimal = true;

    private void Update()
    {
        if (ultimateController == null)
            return;

        bool isCoolingDown = ultimateController.IsOnCooldown;
        float remaining = ultimateController.RemainingCooldown;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.SetActive(isCoolingDown);
        }

        if (cooldownFillImage != null)
        {
            cooldownFillImage.fillAmount = ultimateController.CooldownNormalized;
        }

        if (cooldownText != null)
        {
            if (!isCoolingDown)
            {
                cooldownText.text = "";
            }
            else if (showDecimal)
            {
                cooldownText.text = remaining.ToString("F1");
            }
            else
            {
                cooldownText.text = Mathf.CeilToInt(remaining).ToString();
            }
        }
    }
}
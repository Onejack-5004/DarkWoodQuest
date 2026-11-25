
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    [Header("UI Sound Settings")]
    public string hoverSound = "UI_Hover";
    public string clickSound = "UI_Click";

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // เมื่อเมาส์เลื่อนเข้ามาบนปุ่ม
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable)
            return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(hoverSound);
        }
    }

    // เมื่อคลิกปุ่ม
    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && !button.interactable)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);
        }
    }
}


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class BossIntroDialogue : MonoBehaviour
{
    public enum Speaker
    {
        Boss,
        Player,
        Player2
    }

    [Serializable]
    public class DialogueLine
    {
        public Speaker speaker;

        [TextArea(2, 4)]
        public string text;
    }

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public Image bossPortraitImage;
    public Image playerPortraitImage;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueIndicator;

    [Header("Player References")]
    public CharacterSetup player1Setup;
    public CharacterSetup player2Setup;

    [Header("Boss Information")]
    public string bossName = "BOSS";
    public Sprite bossPortrait;

    [Header("Portrait Direction")]
    public bool bossFlipX = false;
    public bool playerFlipX = true;

    [Header("Dialogue Settings")]
    public DialogueLine[] dialogueLines;
    public float typingSpeed = 0.035f;

    private int currentLine = 0;

    private bool isTyping = false;
    private bool dialogueStarted = false;

    private Coroutine typingCoroutine;

    public Action OnDialogueFinished;

    private void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (bossPortraitImage != null)
        {
            bossPortraitImage.gameObject.SetActive(false);
        }

        if (playerPortraitImage != null)
        {
            playerPortraitImage.gameObject.SetActive(false);
        }

        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        if (!dialogueStarted)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleSpacePressed();
        }
    }

    public void BeginDialogue()
    {
        if (dialogueStarted)
            return;

        if (dialoguePanel == null ||
            dialogueText == null ||
            speakerNameText == null)
        {
            Debug.LogError("Dialogue UI is missing!");
            return;
        }

        if (dialogueLines == null ||
            dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines found!");
            FinishDialogue();
            return;
        }

        dialogueStarted = true;
        currentLine = 0;

        dialoguePanel.SetActive(true);

        ShowCurrentLine();

        Debug.Log("BOSS INTRO DIALOGUE STARTED!");
    }

    private void HandleSpacePressed()
    {
        // กำลังพิมพ์ข้อความ
        // กด Space เพื่อแสดงข้อความทั้งหมด
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            dialogueText.text =
                dialogueLines[currentLine].text;

            isTyping = false;

            UpdateContinueIndicator();

            return;
        }

        // ไปยังบทสนทนาถัดไป
        currentLine++;

        if (currentLine >= dialogueLines.Length)
        {
            FinishDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        DialogueLine line = dialogueLines[currentLine];

        UpdateSpeaker(line.speaker);

        dialogueText.text = "";

        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }

        typingCoroutine = StartCoroutine(TypeLine());
    }

    private void UpdateSpeaker(Speaker speaker)
    {
        // ซ่อน Portrait ทั้งสองก่อน
        if (bossPortraitImage != null)
        {
            bossPortraitImage.gameObject.SetActive(false);
        }

        if (playerPortraitImage != null)
        {
            playerPortraitImage.gameObject.SetActive(false);
        }

        CharacterData playerData = null;

        switch (speaker)
        {
            case Speaker.Boss:

                speakerNameText.text = bossName;

                ShowBossPortrait();

                break;

            case Speaker.Player:

                if (player1Setup != null)
                {
                    playerData = player1Setup.characterData;
                }

                ShowPlayerPortrait(playerData);

                break;

            case Speaker.Player2:

                if (player2Setup != null)
                {
                    playerData = player2Setup.characterData;
                }

                ShowPlayerPortrait(playerData);

                break;
        }
    }

    private void ShowBossPortrait()
    {
        if (bossPortraitImage == null)
            return;

        bossPortraitImage.sprite = bossPortrait;

        bossPortraitImage.gameObject.SetActive(
            bossPortrait != null
        );

        SetPortraitDirection(
            bossPortraitImage,
            bossFlipX
        );
    }

    private void ShowPlayerPortrait(CharacterData data)
    {
        if (data == null)
        {
            speakerNameText.text = "PLAYER";

            Debug.LogWarning(
                "Player CharacterData is missing!"
            );

            return;
        }

        // ดึงชื่อตัวละครจาก CharacterData
        speakerNameText.text = data.characterName;

        if (playerPortraitImage == null)
            return;

        // ดึง Portrait จาก CharacterData
        playerPortraitImage.sprite = data.dialoguePortrait;

        playerPortraitImage.gameObject.SetActive(
            data.dialoguePortrait != null
        );

        SetPortraitDirection(
            playerPortraitImage,
            playerFlipX
        );
    }

    private void SetPortraitDirection(
        Image portrait,
        bool flipX
    )
    {
        if (portrait == null)
            return;

        Vector3 scale =
            portrait.rectTransform.localScale;

        scale.x = Mathf.Abs(scale.x);

        if (flipX)
        {
            scale.x *= -1f;
        }

        portrait.rectTransform.localScale = scale;
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;

        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }

        string line = dialogueLines[currentLine].text;

        foreach (char letter in line)
        {
            dialogueText.text += letter;

            yield return new WaitForSecondsRealtime(
                typingSpeed
            );
        }

        isTyping = false;

        typingCoroutine = null;

        UpdateContinueIndicator();
    }

    private void UpdateContinueIndicator()
    {
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(!isTyping);
        }
    }

    private void FinishDialogue()
    {
        dialogueStarted = false;
        isTyping = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // ซ่อน Portrait หลังจบบทสนทนา
        if (bossPortraitImage != null)
        {
            bossPortraitImage.gameObject.SetActive(false);
        }

        if (playerPortraitImage != null)
        {
            playerPortraitImage.gameObject.SetActive(false);
        }

        Debug.Log("BOSS INTRO DIALOGUE FINISHED!");

        OnDialogueFinished?.Invoke();
    }
}

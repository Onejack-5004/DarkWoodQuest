
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData[] characters;

    [Header("Character Select Panel")]
    public GameObject characterSelectPanel;
    public GameObject player2Section;

    [Header("Player 1 UI")]
    public Image player1Image;
    public TMP_Text player1Name;
    public TMP_Text player1ConfirmText;

    [Header("Player 2 UI")]
    public Image player2Image;
    public TMP_Text player2Name;
    public TMP_Text player2ConfirmText;

    [Header("Stage Settings")]
    public string stageSceneName = "Stage_01";

    private int player1Index = 0;
    private int player2Index = 0;

    private bool player1Confirmed = false;
    private bool player2Confirmed = false;

    private bool readyMessageShown = false;

    private void Start()
    {
        ResetSelection();
    }

    private void Update()
    {
        if (characterSelectPanel == null ||
            !characterSelectPanel.activeInHierarchy)
            return;

        if (Keyboard.current == null)
            return;

        // Player 1
        if (Keyboard.current.aKey.wasPressedThisFrame)
            Player1Previous();

        if (Keyboard.current.dKey.wasPressedThisFrame)
            Player1Next();

        if (Keyboard.current.wKey.wasPressedThisFrame)
            TogglePlayer1Confirm();

        // Player 2
        if (GameSession.isDuoPlay &&
            player2Section != null &&
            player2Section.activeSelf)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                Player2Previous();

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                Player2Next();

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                TogglePlayer2Confirm();
        }
    }

    // รีเซ็ตสถานะการเลือกตัวละครทั้งหมด
    public void ResetSelection()
    {
        player1Index = 0;
        player2Index = 0;

        player1Confirmed = false;
        player2Confirmed = false;

        readyMessageShown = false;

        GameSession.player1Character = null;
        GameSession.player2Character = null;

        UpdatePlayer1Display();
        UpdatePlayer2Display();
        UpdateConfirmDisplay();

        Debug.Log("Character Selection Reset");
    }

    public void Player1Previous()
    {
        if (player1Confirmed || !HasCharacters())
            return;

        player1Index--;

        if (player1Index < 0)
            player1Index = characters.Length - 1;

        UpdatePlayer1Display();
    }

    public void Player1Next()
    {
        if (player1Confirmed || !HasCharacters())
            return;

        player1Index++;

        if (player1Index >= characters.Length)
            player1Index = 0;

        UpdatePlayer1Display();
    }

    public void TogglePlayer1Confirm()
    {
        if (!HasValidCharacter(player1Index))
            return;

        player1Confirmed = !player1Confirmed;

        if (player1Confirmed)
        {
            GameSession.player1Character = characters[player1Index];

            Debug.Log("P1 Selected: " +
                      GameSession.player1Character.characterName);
        }
        else
        {
            GameSession.player1Character = null;

            Debug.Log("P1 Unconfirmed");
        }

        UpdateConfirmDisplay();
        CheckAllPlayersConfirmed();
    }

    public void Player2Previous()
    {
        if (player2Confirmed || !HasCharacters())
            return;

        player2Index--;

        if (player2Index < 0)
            player2Index = characters.Length - 1;

        UpdatePlayer2Display();
    }

    public void Player2Next()
    {
        if (player2Confirmed || !HasCharacters())
            return;

        player2Index++;

        if (player2Index >= characters.Length)
            player2Index = 0;

        UpdatePlayer2Display();
    }

    public void TogglePlayer2Confirm()
    {
        if (!GameSession.isDuoPlay ||
            player2Section == null ||
            !player2Section.activeSelf ||
            !HasValidCharacter(player2Index))
            return;

        player2Confirmed = !player2Confirmed;

        if (player2Confirmed)
        {
            GameSession.player2Character = characters[player2Index];

            Debug.Log("P2 Selected: " +
                      GameSession.player2Character.characterName);
        }
        else
        {
            GameSession.player2Character = null;

            Debug.Log("P2 Unconfirmed");
        }

        UpdateConfirmDisplay();
        CheckAllPlayersConfirmed();
    }

    private void UpdatePlayer1Display()
    {
        if (!HasValidCharacter(player1Index))
            return;

        CharacterData data = characters[player1Index];

        player1Name.text = data.characterName;
        player1Image.sprite = data.characterIcon;
        player1Image.enabled = data.characterIcon != null;
    }

    private void UpdatePlayer2Display()
    {
        if (!HasValidCharacter(player2Index))
            return;

        CharacterData data = characters[player2Index];

        player2Name.text = data.characterName;
        player2Image.sprite = data.characterIcon;
        player2Image.enabled = data.characterIcon != null;
    }

    private void UpdateConfirmDisplay()
    {
        if (player1ConfirmText != null)
        {
            player1ConfirmText.text = player1Confirmed
                ? "CONFIRMED!"
                : "PRESS [W] TO CONFIRM";
        }

        if (player2ConfirmText != null)
        {
            player2ConfirmText.text = player2Confirmed
                ? "CONFIRMED!"
                : "PRESS [UP] TO CONFIRM";
        }
    }

    private bool HasCharacters()
    {
        return characters != null && characters.Length > 0;
    }

    private bool HasValidCharacter(int index)
    {
        return HasCharacters() &&
               index >= 0 &&
               index < characters.Length &&
               characters[index] != null;
    }

    private void CheckAllPlayersConfirmed()
    {
        bool allConfirmed;

        if (GameSession.isDuoPlay)
        {
            allConfirmed =
                player1Confirmed &&
                player2Confirmed;
        }
        else
        {
            allConfirmed = player1Confirmed;
        }

        if (allConfirmed && !readyMessageShown)
        {
            readyMessageShown = true;

            Debug.Log("ALL PLAYERS CONFIRMED! STARTING GAME!");

            StartGame();
        }

        if (!allConfirmed)
        {
            readyMessageShown = false;
        }
    }
    private void StartGame()
    {
        if (GameSession.player1Character == null)
        {
            Debug.LogError("Player 1 has not selected a character!");
            return;
        }

        if (GameSession.isDuoPlay &&
            GameSession.player2Character == null)
        {
            Debug.LogError("Player 2 has not selected a character!");
            return;
        }

        Debug.Log("Loading Stage: " + stageSceneName);

        SceneManager.LoadScene(stageSceneName);
    }
}

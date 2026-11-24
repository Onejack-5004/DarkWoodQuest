
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;

    [Header("Character Select")]
    public GameObject characterSelectPanel;
    public GameObject player1Section;
    public GameObject player2Section;

    [Header("Character Select Manager")]
    public CharacterSelectManager characterSelectManager;

    private RectTransform player1Rect;

    private void Awake()
    {
        player1Rect = player1Section.GetComponent<RectTransform>();
    }

    private void Start()
    {
        ShowMainMenu();
    }

    public void SelectSinglePlay()
    {
        GameSession.ResetSession();
        GameSession.isDuoPlay = false;

        characterSelectManager.ResetSelection();

        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);

        player2Section.SetActive(false);

        SetPlayer1Layout(true);

        Debug.Log("Game Mode: SINGLE PLAY");
    }

    public void SelectDuoPlay()
    {
        GameSession.ResetSession();
        GameSession.isDuoPlay = true;

        characterSelectManager.ResetSelection();

        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);

        player2Section.SetActive(true);

        SetPlayer1Layout(false);

        Debug.Log("Game Mode: DUO PLAY");
    }

    private void SetPlayer1Layout(bool singlePlay)
    {
        if (singlePlay)
        {
            player1Rect.anchorMin = new Vector2(0f, 0f);
            player1Rect.anchorMax = new Vector2(1f, 1f);
        }
        else
        {
            player1Rect.anchorMin = new Vector2(0f, 0f);
            player1Rect.anchorMax = new Vector2(0.5f, 1f);
        }

        player1Rect.offsetMin = Vector2.zero;
        player1Rect.offsetMax = Vector2.zero;
    }

    public void ShowMainMenu()
    {
        characterSelectManager.ResetSelection();

        characterSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        player2Section.SetActive(false);
    }
}

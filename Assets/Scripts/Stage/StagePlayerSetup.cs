
using UnityEngine;

public class StagePlayerSetup : MonoBehaviour
{
    [Header("Player Objects")]
    public GameObject player1;
    public GameObject player2;

    private void Awake()
    {
        SetupPlayers();
    }

    private void SetupPlayers()
    {
        // ตรวจสอบ Player 1
        if (player1 == null)
        {
            Debug.LogError("Player 1 is not assigned!");
            return;
        }

        // ตรวจสอบข้อมูลตัวละคร P1
        if (GameSession.player1Character == null)
        {
            Debug.LogError("Player 1 Character Data is missing!");
            return;
        }

        // ตั้งค่าตัวละคร P1
        player1.SetActive(true);

        CharacterSetup setup1 =
            player1.GetComponent<CharacterSetup>();

        if (setup1 != null)
        {
            setup1.characterData = GameSession.player1Character;
            setup1.ApplyCharacterData(GameSession.player1Character);
        }
        else
        {
            Debug.LogError("CharacterSetup is missing on Player 1!");
        }

        // ตั้งค่า Player 2 ตามโหมดการเล่น
        if (player2 == null)
        {
            Debug.LogError("Player 2 is not assigned!");
            return;
        }

        if (GameSession.isDuoPlay)
        {
            if (GameSession.player2Character == null)
            {
                Debug.LogError("Player 2 Character Data is missing!");
                return;
            }

            player2.SetActive(true);

            CharacterSetup setup2 =
                player2.GetComponent<CharacterSetup>();

            if (setup2 != null)
            {
                setup2.characterData = GameSession.player2Character;
                setup2.ApplyCharacterData(GameSession.player2Character);
            }
            else
            {
                Debug.LogError("CharacterSetup is missing on Player 2!");
            }

            Debug.Log("DUO PLAY SETUP COMPLETE!");
        }
        else
        {
            player2.SetActive(false);

            Debug.Log("SINGLE PLAY SETUP COMPLETE!");
        }
    }
}

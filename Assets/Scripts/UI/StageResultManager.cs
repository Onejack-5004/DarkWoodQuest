
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageResultManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject stageResultPanel;
    public GameObject gameOverPanel;

    [Header("Player References")]
    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    [Header("Boss Reference")]
    public BossHealth bossHealth;

    private bool hasFinished = false;

    private void Start()
    {
        Time.timeScale = 1f;

        stageResultPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (bossHealth != null)
        {
            bossHealth.OnBossDied += ShowStageClear;
        }

        if (player1Health != null)
        {
            player1Health.OnPlayerDied += CheckGameOver;
        }

        if (player2Health != null)
        {
            player2Health.OnPlayerDied += CheckGameOver;
        }
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
            bossHealth.OnBossDied -= ShowStageClear;

        if (player1Health != null)
            player1Health.OnPlayerDied -= CheckGameOver;

        if (player2Health != null)
            player2Health.OnPlayerDied -= CheckGameOver;
    }

    private void ShowStageClear()
    {
        if (hasFinished)
            return;

        hasFinished = true;

        stageResultPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        Time.timeScale = 0f;

        Debug.Log("STAGE 1 CLEAR!");
    }

    private void CheckGameOver()
    {
        if (hasFinished)
            return;

        bool player1Dead =
            player1Health == null ||
            player1Health.isDead;

        bool player2Dead =
            player2Health == null ||
            player2Health.isDead;

        bool allPlayersDead;

        if (GameSession.isDuoPlay)
        {
            allPlayersDead = player1Dead && player2Dead;
        }
        else
        {
            allPlayersDead = player1Dead;
        }

        if (allPlayersDead)
        {
            ShowGameOver();
        }
    }

    private void ShowGameOver()
    {
        if (hasFinished)
            return;

        hasFinished = true;

        gameOverPanel.SetActive(true);
        stageResultPanel.SetActive(false);

        Time.timeScale = 0f;

        Debug.Log("GAME OVER!");
    }

    public void NextStage()
    {
        Debug.Log("NEXT STAGE IS COMING SOON!");
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Stage_01");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}

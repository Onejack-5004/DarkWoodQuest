
using UnityEngine;
using System.Collections;

public class StageAreaTransition : MonoBehaviour
{
    [Header("Fade System")]
    public ScreenFader screenFader;

    [Header("Area References")]
    public GameObject waveArea;
    public GameObject bossArea;

    [Header("Player References")]
    public GameObject player1;
    public GameObject player2;

    [Header("Boss Entry Points")]
    public Transform entryPointP1;
    public Transform entryPointP2;

    [Header("Boss Start Points")]
    public Transform startPointP1;
    public Transform startPointP2;

    [Header("Walking Settings")]
    public float entryDirection = -1f;
    public float arrivalDistance = 0.15f;

    [Header("Transition Settings")]
    public float blackScreenDelay = 2f;

    [Header("Boss Intro")]
    public BossController bossController;
    public BossIntroDialogue bossIntroDialogue;

    public BossHealthUI bossHealthUI;

    private bool hasTransitioned = false;
    private bool isWalkingIntoBoss = false;

    private void OnEnable()
    {
        if (screenFader != null)
        {
            screenFader.OnFadeOutComplete += SwitchToBossArea;
            screenFader.OnFadeInComplete += StartBossEntry;
        }
        if (bossIntroDialogue != null)
        {
            bossIntroDialogue.OnDialogueFinished += BeginBossFight;
        }
    }

    private void OnDisable()
    {
        if (screenFader != null)
        {
            screenFader.OnFadeOutComplete -= SwitchToBossArea;
            screenFader.OnFadeInComplete -= StartBossEntry;
        }
        if (bossIntroDialogue != null)
        {
            bossIntroDialogue.OnDialogueFinished -= BeginBossFight;
        }
    }

    private void Update()
    {
        if (!isWalkingIntoBoss)
            return;

        bool player1Arrived = CheckPlayerArrival(
            player1,
            startPointP1
        );

        bool player2Arrived = CheckPlayerArrival(
            player2,
            startPointP2
        );

        if (player1Arrived && player2Arrived)
        {
            isWalkingIntoBoss = false;

            Debug.Log("PLAYERS ARRIVED AT BOSS AREA!");

            // เริ่มบทสนทนา
            if (bossIntroDialogue != null)
            {
                bossIntroDialogue.BeginDialogue();
            }
            else
            {
                Debug.LogError("Boss Intro Dialogue is missing!");
                BeginBossFight();
            }
        }
    }

    private void SwitchToBossArea()
    {
        if (hasTransitioned)
            return;

        hasTransitioned = true;

        Debug.Log("SWITCHING TO BOSS AREA!");

        // ปิดพื้นที่ Wave
        if (waveArea != null)
        {
            waveArea.SetActive(false);
        }

        // เปิดพื้นที่ Boss
        if (bossArea != null)
        {
            bossArea.SetActive(true);
        }

        // ย้ายผู้เล่นไปยังจุดเกิด
        PreparePlayerForEntry(
            player1,
            entryPointP1
        );

        PreparePlayerForEntry(
            player2,
            entryPointP2
        );

        Debug.Log("BOSS AREA READY!");

        // รอก่อนเปิดภาพ Boss Area
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
        Debug.Log(
            "BLACK SCREEN DELAY: " +
            blackScreenDelay +
            " seconds"
        );

        yield return new WaitForSecondsRealtime(
            blackScreenDelay
        );

        screenFader.FadeIn();
    }

    private void PreparePlayerForEntry(
        GameObject player,
        Transform entryPoint
    )
    {
        if (player == null || !player.activeInHierarchy)
            return;

        PlayerHealth health =
            player.GetComponent<PlayerHealth>();

        // ไม่ย้ายผู้เล่นที่ตายแล้ว
        if (health == null || health.isDead)
            return;

        if (entryPoint == null)
        {
            Debug.LogError(
                "Entry Point missing for " +
                player.name
            );

            return;
        }

        PlayerController controller =
            player.GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.StopAutoWalk();
            controller.SetControlEnabled(false);
        }

        Rigidbody2D rb =
            player.GetComponent<Rigidbody2D>();

        // ย้ายตำแหน่งผู้เล่น
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = entryPoint.position;
        }
        else
        {
            player.transform.position =
                entryPoint.position;
        }
    }

    private void StartBossEntry()
    {
        if (!hasTransitioned)
            return;

        Debug.Log("PLAYERS ENTERING BOSS AREA!");

        isWalkingIntoBoss = true;

        StartPlayerEntry(player1);
        StartPlayerEntry(player2);
    }

    private void StartPlayerEntry(GameObject player)
    {
        if (player == null || !player.activeInHierarchy)
            return;

        PlayerHealth health =
            player.GetComponent<PlayerHealth>();

        if (health == null || health.isDead)
            return;

        PlayerController controller =
            player.GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.StartAutoWalk(entryDirection);
        }
    }

    private bool CheckPlayerArrival(
        GameObject player,
        Transform startPoint
    )
    {
        // ไม่มีผู้เล่น หรือผู้เล่นไม่ได้เปิดใช้งาน
        if (player == null || !player.activeInHierarchy)
            return true;

        PlayerHealth health =
            player.GetComponent<PlayerHealth>();

        // ผู้เล่นที่ตายแล้วไม่ต้องรอ
        if (health == null || health.isDead)
            return true;

        if (startPoint == null)
            return false;

        float distance = Mathf.Abs(
            player.transform.position.x -
            startPoint.position.x
        );

        if (distance <= arrivalDistance)
        {
            PlayerController controller =
                player.GetComponent<PlayerController>();

            if (controller != null)
            {
                controller.StopAutoWalk();
                controller.SetControlEnabled(false);
            }

            Rigidbody2D rb =
                player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
            }

            return true;
        }

        return false;
    }
    private void BeginBossFight()
    {
        // แสดง Boss HP หลัง Dialogue จบ
        if (bossHealthUI != null)
        {
            bossHealthUI.ShowBossHP();
        }

        // คืนการควบคุมให้ผู้เล่น
        if (player1 != null &&
            player1.GetComponent<PlayerHealth>() != null &&
            !player1.GetComponent<PlayerHealth>().isDead)
        {
            player1.GetComponent<PlayerController>()
                .SetControlEnabled(true);
        }

        if (player2 != null &&
            player2.activeInHierarchy &&
            player2.GetComponent<PlayerHealth>() != null &&
            !player2.GetComponent<PlayerHealth>().isDead)
        {
            player2.GetComponent<PlayerController>()
                .SetControlEnabled(true);
        }

        // เริ่มการต่อสู้
        if (bossController != null)
        {
            bossController.StartBattle();
        }
    }

    private void UnlockPlayer(GameObject player)
    {
        if (player == null || !player.activeInHierarchy)
            return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health == null || health.isDead)
            return;

        PlayerController controller =
            player.GetComponent<PlayerController>();

        if (controller != null)
        {
            controller.SetControlEnabled(true);
        }
    }
}

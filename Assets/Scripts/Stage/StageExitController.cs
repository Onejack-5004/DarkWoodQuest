
using UnityEngine;
using System;

public class StageExitController : MonoBehaviour
{
    [Header("Wave Manager")]
    public WaveManager waveManager;

    [Header("Player References")]
    public GameObject player1;
    public GameObject player2;

    [Header("Exit Settings")]
    public float exitDirection = 1f;
    public float screenExitMargin = 0.05f;

    private Camera mainCamera;

    private bool isExitStarted = false;
    private bool hasAllPlayersExited = false;

    public event Action OnAllLivingPlayersExited;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared +=
                StartPlayerExit;
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared -=
                StartPlayerExit;
        }
    }

    private void Update()
    {
        if (!isExitStarted || hasAllPlayersExited)
            return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
                return;
        }

        int alivePlayerCount = 0;
        bool allExited = true;

        CheckPlayerExit(
            player1,
            ref alivePlayerCount,
            ref allExited
        );

        CheckPlayerExit(
            player2,
            ref alivePlayerCount,
            ref allExited
        );

        if (alivePlayerCount > 0 && allExited)
        {
            hasAllPlayersExited = true;

            Debug.Log("ALL LIVING PLAYERS EXITED!");

            OnAllLivingPlayersExited?.Invoke();
        }
    }

    private void StartPlayerExit()
    {
        if (isExitStarted)
            return;

        isExitStarted = true;

        Debug.Log("PLAYERS ARE WALKING TO THE EXIT!");

        StartPlayerWalking(player1);
        StartPlayerWalking(player2);
    }

    private void StartPlayerWalking(GameObject player)
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
            controller.StartAutoWalk(exitDirection);
        }
        else
        {
            Debug.LogError(
                player.name + " is missing PlayerController!"
            );
        }
    }

    private void CheckPlayerExit(
        GameObject player,
        ref int alivePlayerCount,
        ref bool allExited
    )
    {
        if (player == null || !player.activeInHierarchy)
            return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health == null || health.isDead)
            return;

        alivePlayerCount++;

        Vector3 viewportPosition =
            mainCamera.WorldToViewportPoint(player.transform.position);

        bool hasExited =
            viewportPosition.x > 1f + screenExitMargin;

        if (!hasExited)
        {
            allExited = false;
        }
    }
}

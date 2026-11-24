
using UnityEngine;
using System;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Monster Spawner")]
    public MonsterSpawner monsterSpawner;

    [Header("Wave Settings")]
    public int totalWaves = 3;
    public float delayBetweenWaves = 2f;

    [Header("Monster A Count")]
    public int[] monsterACount = { 3, 4, 5 };

    [Header("Monster B Count")]
    public int[] monsterBCount = { 0, 1, 2 };

    [Header("Current Wave")]
    public int currentWave = 0;

    private bool isHandlingWaveClear = false;
    private bool hasClearedAllWaves = false;

    public event Action OnAllWavesCleared;

    private void OnEnable()
    {
        if (monsterSpawner != null)
        {
            monsterSpawner.OnAllMonstersDefeated +=
                HandleAllMonstersDefeated;
        }
    }

    private void OnDisable()
    {
        if (monsterSpawner != null)
        {
            monsterSpawner.OnAllMonstersDefeated -=
                HandleAllMonstersDefeated;
        }
    }

    private void Start()
    {
        if (monsterSpawner == null)
        {
            Debug.LogError("MonsterSpawner is missing!");
            return;
        }

        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            if (!hasClearedAllWaves)
            {
                hasClearedAllWaves = true;

                Debug.Log("STAGE 1 - ALL WAVES CLEARED!");

                OnAllWavesCleared?.Invoke();
            }

            return;
        }

        int waveIndex = currentWave;

        int amountA = waveIndex < monsterACount.Length
            ? monsterACount[waveIndex]
            : 0;

        int amountB = waveIndex < monsterBCount.Length
            ? monsterBCount[waveIndex]
            : 0;

        currentWave++;

        Debug.Log("========== WAVE " + currentWave + " ==========");

        monsterSpawner.SpawnWave(amountA, amountB);
    }

    private void HandleAllMonstersDefeated()
    {
        if (isHandlingWaveClear || hasClearedAllWaves)
            return;

        isHandlingWaveClear = true;

        Debug.Log("WAVE " + currentWave + " CLEARED!");

        StartCoroutine(NextWaveRoutine());
    }

    private IEnumerator NextWaveRoutine()
    {
        yield return new WaitForSeconds(delayBetweenWaves);

        isHandlingWaveClear = false;

        StartNextWave();
    }
}

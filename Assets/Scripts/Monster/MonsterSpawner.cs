
using UnityEngine;
using System;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Monster Prefabs")]
    public GameObject monsterAPrefab;
    public GameObject monsterBPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Player References")]
    public Transform player1;
    public Transform player2;

    [Header("Spawn Information")]
    public int aliveMonsterCount = 0;

    // แจ้ง WaveManager เมื่อมอนสเตอร์ตายครบ
    public event Action OnAllMonstersDefeated;

    public void SpawnWave(int monsterACount, int monsterBCount)
    {
        if (aliveMonsterCount > 0)
        {
            Debug.LogWarning(
                "Cannot spawn a new wave while monsters are alive!"
            );
            return;
        }

        aliveMonsterCount = 0;

        Debug.Log("Spawning Monsters...");

        for (int i = 0; i < monsterACount; i++)
        {
            SpawnMonster(monsterAPrefab);
        }

        for (int i = 0; i < monsterBCount; i++)
        {
            SpawnMonster(monsterBPrefab);
        }

        Debug.Log(
            "Wave Spawned! Total Monsters: " +
            aliveMonsterCount
        );

        if (aliveMonsterCount == 0)
        {
            OnAllMonstersDefeated?.Invoke();
        }
    }

    private void SpawnMonster(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Monster Prefab is missing!");
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(
                0,
                spawnPoints.Length
            );

            if (spawnPoints[randomIndex] != null)
            {
                spawnPosition =
                    spawnPoints[randomIndex].position;
            }
        }

        GameObject monster = Instantiate(
            prefab,
            spawnPosition,
            Quaternion.identity
        );

        MonsterFollow follow =
            monster.GetComponent<MonsterFollow>();

        MonsterHealth health =
            monster.GetComponent<MonsterHealth>();

        if (follow == null || health == null)
        {
            Debug.LogError(
                "Monster Prefab needs MonsterFollow and MonsterHealth!"
            );

            Destroy(monster);
            return;
        }

        // ส่ง Player References ให้มอนสเตอร์
        follow.player1 = player1;

        // ไม่ส่ง Player 2 หากอยู่ใน Single Play
        follow.player2 =
            player2 != null &&
            player2.gameObject.activeInHierarchy
            ? player2
            : null;

        // เพิ่มจำนวนมอนสเตอร์ที่ยังมีชีวิต
        aliveMonsterCount++;

        // ติดตามการตายของมอนสเตอร์
        health.OnDied += HandleMonsterDied;
    }

    private void HandleMonsterDied(MonsterHealth monster)
    {
        aliveMonsterCount--;

        aliveMonsterCount = Mathf.Max(
            aliveMonsterCount,
            0
        );

        Debug.Log(
            "Remaining Monsters: " + aliveMonsterCount
        );

        if (aliveMonsterCount <= 0)
        {
            Debug.Log("ALL MONSTERS DEFEATED!");

            OnAllMonstersDefeated?.Invoke();
        }
    }
}

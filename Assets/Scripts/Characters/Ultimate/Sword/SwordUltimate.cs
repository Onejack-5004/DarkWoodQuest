
using UnityEngine;
using System.Collections;

public class SwordUltimate : PlayerUltimateAbility
{
    [Header("Sword Wave")]
    public GameObject swordWavePrefab;
    public int damage = 50;
    public float waveSpeed = 12f;
    public float waveLifetime = 1.5f;
    public float spawnOffset = 1f;

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    public override UltimateType AbilityType => UltimateType.Sword;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override IEnumerator ExecuteUltimate()
    {
        if (swordWavePrefab == null)
        {
            Debug.LogError("Sword Wave Prefab is missing!");
            yield break;
        }

        // ตรวจสอบทิศที่ตัวละครกำลังหัน
        float direction = 1f;

        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            direction = -1f;
        }

        // กำหนดตำแหน่งปล่อยคลื่นดาบ
        Vector3 spawnPosition = transform.position;
        spawnPosition.x += spawnOffset * direction;

        // สร้างคลื่นดาบ
        GameObject wave = Instantiate(
            swordWavePrefab,
            spawnPosition,
            Quaternion.identity
        );

        SwordWaveProjectile projectile =
            wave.GetComponent<SwordWaveProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(
                direction,
                waveSpeed,
                damage,
                waveLifetime
            );
        }
        else
        {
            Debug.LogError(
                "SwordWaveProjectile component is missing on Prefab!"
            );
            Destroy(wave);
            yield break;
        }

        Debug.Log("Sword Ultimate: Sword Wave!");

        // ล็อกการเคลื่อนที่จนกว่าคลื่นดาบจะหมดอายุ
        yield return new WaitForSeconds(waveLifetime);
    }
}

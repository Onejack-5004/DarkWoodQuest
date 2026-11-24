
using UnityEngine;
using System.Collections;

public class GunUltimate : PlayerUltimateAbility
{
    [Header("Laser Beam")]
    public GameObject laserBeamPrefab;

    public int damage = 80;

    public float chargeDuration = 1f;
    public float beamDuration = 0.4f;

    public float beamLength = 12f;
    public float beamThickness = 1f;

    public float spawnOffset = 0.5f;

    private SpriteRenderer spriteRenderer;

    public override UltimateType AbilityType => UltimateType.Gun;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override IEnumerator ExecuteUltimate()
    {
        if (laserBeamPrefab == null)
        {
            Debug.LogError("Laser Beam Prefab is missing!");
            yield break;
        }

        // ตรวจสอบทิศทางที่ตัวละครหัน
        float direction = 1f;

        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            direction = -1f;
        }

        Debug.Log("Gun Ultimate: Charging...");

        // ชาร์จพลังงาน
        yield return new WaitForSeconds(chargeDuration);

        // หาตำแหน่งกึ่งกลางของลำแสง
        Vector3 spawnPosition = transform.position;

        spawnPosition.x += direction *
            (beamLength / 2f + spawnOffset);

        // สร้าง Laser Beam
        GameObject beam = Instantiate(
            laserBeamPrefab,
            spawnPosition,
            Quaternion.identity
        );

        LaserBeamProjectile laser =
            beam.GetComponent<LaserBeamProjectile>();

        if (laser != null)
        {
            laser.Initialize(
                direction,
                damage,
                beamLength,
                beamThickness,
                beamDuration
            );
        }
        else
        {
            Debug.LogError(
                "LaserBeamProjectile component is missing!"
            );

            Destroy(beam);
            yield break;
        }

        Debug.Log("Gun Ultimate: LASER FIRED!");

        // รอจนกว่าลำแสงจะหมดอายุ
        yield return new WaitForSeconds(beamDuration);
    }
}

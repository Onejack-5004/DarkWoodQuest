
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    [Header("Firing Point")]
    public Transform firePoint;

    [Header("Hit Detection")]
    public LayerMask monsterLayer;
    public LayerMask bossLayer;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Fire(WeaponData weaponData)
    {
        if (weaponData == null ||
            weaponData.projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab is missing!");
            return;
        }

        float direction = 1f;

        if (spriteRenderer != null && spriteRenderer.flipX)
            direction = -1f;

        Vector3 spawnPosition = transform.position;

        if (firePoint != null)
        {
            Vector3 localPosition =
                transform.InverseTransformPoint(firePoint.position);

            localPosition.x =
                Mathf.Abs(localPosition.x) * direction;

            spawnPosition =
                transform.TransformPoint(localPosition);
        }

        GameObject projectileObject = Instantiate(
            weaponData.projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "Projectile script is missing on Prefab!"
            );

            Destroy(projectileObject);
            return;
        }

        // ส่ง Layer ของทั้ง Monster และ Boss
        LayerMask targetLayer = monsterLayer | bossLayer;

        projectile.Initialize(
            direction,
            weaponData,
            targetLayer
        );
    }
}

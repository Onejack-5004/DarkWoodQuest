
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewWeaponData",
    menuName = "DarkWood Quest/Weapon Data"
)]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Information")]
    public string weaponName;
    public Sprite weaponIcon;

    [Header("Attack Settings")]
    public AttackType attackType = AttackType.Hitscan;
    public int attackDamage = 15;
    public float attackCooldown = 0.25f;
    public float attackRange = 10f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;

    [Header("Explosion Settings")]
    public int explosionDamage = 25;
    public float explosionRadius = 2f;

    [Header("Shotgun Settings")]
    public int pelletCount = 5;
    public float spreadAngle = 20f;
}

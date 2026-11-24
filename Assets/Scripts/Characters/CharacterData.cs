
using UnityEngine;

public enum AttackType
{
    Melee,
    Hitscan,
    Projectile
}

public enum UltimateType
{
    Sword,
    Gun,
    Magic,
    Fist
}

[CreateAssetMenu(
    fileName = "NewCharacterData",
    menuName = "DarkWood Quest/Character Data"
)]
public class CharacterData : ScriptableObject
{
    [Header("Character Information")]
    public string characterName;
    public Sprite characterIcon;

    [Header("Dialogue Portrait")]
    public Sprite dialoguePortrait;

    [Header("Movement Stats")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Health Stats")]
    public int maxHealth = 100;

    [Header("Combat Settings")]
    public AttackType attackType = AttackType.Melee;

    public int attackDamage = 20;
    public float attackCooldown = 0.5f;
    public float attackRange = 0.8f;

    [Header("Starting Weapon")]
    public WeaponData startingWeapon;

    [Header("Ultimate Settings")]
    public UltimateType ultimateType;
    public float ultimateCooldown = 12f;

    [Header("Animation Settings")]
    public RuntimeAnimatorController animatorController;
}

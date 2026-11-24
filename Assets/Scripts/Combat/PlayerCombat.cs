
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController playerController;
    private CharacterSetup characterSetup;

    private MeleeAttack meleeAttack;
    private HitscanAttack hitscanAttack;
    private ProjectileAttack projectileAttack;

    private WeaponData currentWeapon;

    private float nextAttackTime = 0f;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        characterSetup = GetComponent<CharacterSetup>();

        meleeAttack = GetComponent<MeleeAttack>();
        hitscanAttack = GetComponent<HitscanAttack>();
        projectileAttack = GetComponent<ProjectileAttack>();
    }

    private void Start()
    {
        if (characterSetup != null &&
            characterSetup.characterData != null)
        {
            currentWeapon =
                characterSetup.characterData.startingWeapon;
        }

        if (playerController != null)
        {
            playerController.OnAttackPressed += BasicAttack;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnAttackPressed -= BasicAttack;
        }
    }

    private void BasicAttack()
    {
        if (characterSetup == null ||
            characterSetup.characterData == null)
        {
            Debug.LogWarning("Character Data is missing!");
            return;
        }

        if (playerController == null) return;

        CharacterData characterData =
            characterSetup.characterData;

        // ห้ามโจมตีระหว่างกระโดดหรือระหว่างสถานะที่ไม่อนุญาต
        if (!playerController.IsGrounded ||
            playerController.IsAttacking ||
            playerController.IsHitstunned ||
            playerController.IsAutoWalking ||
            playerController.IsUltimateLocked ||
            playerController.IsDead ||
            !playerController.canControl)
        {
            return;
        }

        if (Time.time < nextAttackTime) return;

        // ตรวจสอบอุปกรณ์โจมตีก่อนเริ่ม Animation
        if (characterData.attackType == AttackType.Melee)
        {
            if (meleeAttack == null)
            {
                Debug.LogWarning("MeleeAttack is missing!");
                return;
            }

            if (!playerController.TryStartAttackAnimation())
                return;

            nextAttackTime =
                Time.time + characterData.attackCooldown;

            meleeAttack.Attack(
                characterData.attackDamage,
                characterData.attackRange
            );

            return;
        }

        // Ranged Attack
        if (currentWeapon == null)
        {
            Debug.LogWarning("Weapon Data is missing!");
            return;
        }

        if (currentWeapon.attackType == AttackType.Hitscan)
        {
            if (hitscanAttack == null)
            {
                Debug.LogWarning("HitscanAttack is missing!");
                return;
            }

            if (!playerController.TryStartAttackAnimation())
                return;

            nextAttackTime =
                Time.time + currentWeapon.attackCooldown;

            hitscanAttack.Fire(
                currentWeapon.attackDamage,
                currentWeapon.attackRange
            );
        }
        else if (currentWeapon.attackType == AttackType.Projectile)
        {
            if (projectileAttack == null)
            {
                Debug.LogWarning("ProjectileAttack is missing!");
                return;
            }

            if (!playerController.TryStartAttackAnimation())
                return;

            nextAttackTime =
                Time.time + currentWeapon.attackCooldown;

            projectileAttack.Fire(currentWeapon);
        }
        else
        {
            Debug.Log("Weapon Melee is not implemented yet!");
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null) return;

        currentWeapon = newWeapon;
        nextAttackTime = 0f;

        Debug.Log("Equipped Weapon: " + currentWeapon.weaponName);
    }
}

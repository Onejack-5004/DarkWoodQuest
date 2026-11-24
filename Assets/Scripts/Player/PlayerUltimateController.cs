
using UnityEngine;
using System.Collections;

public class PlayerUltimateController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public PlayerHealth playerHealth;
    public CharacterSetup characterSetup;

    private PlayerUltimateAbility ultimateAbility;

    private bool isUsingUltimate = false;
    private float nextUltimateTime = 0f;
    private float lastMaxCooldown = 0f;

    public bool IsUsingUltimate => isUsingUltimate;

    public float RemainingCooldown =>
        Mathf.Max(0f, nextUltimateTime - Time.time);

    public float MaxCooldown
    {
        get
        {
            if (lastMaxCooldown > 0f)
                return lastMaxCooldown;

            if (characterSetup != null &&
                characterSetup.characterData != null)
            {
                return Mathf.Max(
                    0.01f,
                    characterSetup.characterData.ultimateCooldown
                );
            }

            return 1f;
        }
    }

    public bool IsOnCooldown => RemainingCooldown > 0f;

    public float CooldownNormalized =>
        MaxCooldown > 0f
            ? Mathf.Clamp01(RemainingCooldown / MaxCooldown)
            : 0f;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (characterSetup == null)
            characterSetup = GetComponent<CharacterSetup>();
    }

    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnUltimatePressed += TryUseUltimate;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnUltimatePressed -= TryUseUltimate;
        }
    }

    private void Start()
    {
        FindUltimateAbility();
    }

    private void FindUltimateAbility()
    {
        PlayerUltimateAbility[] abilities =
            GetComponents<PlayerUltimateAbility>();

        if (characterSetup == null ||
            characterSetup.characterData == null)
        {
            Debug.LogError(
                gameObject.name + " Character Data is missing!"
            );
            return;
        }

        UltimateType selectedType =
            characterSetup.characterData.ultimateType;

        foreach (PlayerUltimateAbility ability in abilities)
        {
            if (ability.AbilityType == selectedType)
            {
                ultimateAbility = ability;
                break;
            }
        }

        if (ultimateAbility == null)
        {
            Debug.Log(
                gameObject.name +
                " does not have an Ultimate Ability for " +
                selectedType
            );
        }
        else
        {
            Debug.Log(
                gameObject.name +
                " Ultimate Ready: " + selectedType
            );
        }
    }

    private void TryUseUltimate()
    {
        if (isUsingUltimate)
            return;

        if (playerHealth != null && playerHealth.isDead)
            return;

        if (playerController == null ||
            !playerController.canControl ||
            playerController.IsHitstunned ||
            playerController.IsAutoWalking ||
            playerController.IsUltimateLocked ||
            playerController.IsAttacking ||
            !playerController.IsGrounded)
        {
            return;
        }

        if (ultimateAbility == null)
            return;

        if (Time.time < nextUltimateTime)
        {
            float remainingTime =
                nextUltimateTime - Time.time;

            Debug.Log(
                gameObject.name +
                " Ultimate Cooldown: " +
                remainingTime.ToString("F1") +
                " seconds"
            );

            return;
        }

        float cooldown =
            characterSetup.characterData.ultimateCooldown;

        if (cooldown < 0f)
            cooldown = 0f;

        lastMaxCooldown = cooldown;
        nextUltimateTime = Time.time + cooldown;

        StartCoroutine(UltimateRoutine());
    }

    private IEnumerator UltimateRoutine()
    {
        isUsingUltimate = true;

        playerController.SetUltimateLocked(true);
        playerController.PlayUltimateAnimation();

        Debug.Log(
            gameObject.name + " ULTIMATE START!"
        );

        yield return StartCoroutine(
            ultimateAbility.ExecuteUltimate()
        );

        if (playerController != null)
        {
            playerController.SetUltimateLocked(false);
        }

        isUsingUltimate = false;

        Debug.Log(
            gameObject.name + " ULTIMATE END!"
        );
    }
}

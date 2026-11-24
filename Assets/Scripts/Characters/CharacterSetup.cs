
using UnityEngine;

public class CharacterSetup : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;

    private PlayerController controller;
    private PlayerHealth health;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        GetComponents();

        // รองรับการกำหนด CharacterData ผ่าน Inspector
        if (characterData != null)
        {
            ApplyCharacterData(characterData);
        }
    }

    private void GetComponents()
    {
        controller = GetComponent<PlayerController>();
        health = GetComponent<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void ApplyCharacterData(CharacterData data)
    {
        if (data == null)
        {
            Debug.LogError("Character Data is missing on " + gameObject.name);
            return;
        }

        // ป้องกันกรณี Stage เรียกใช้ก่อน Awake ของ CharacterSetup
        GetComponents();

        characterData = data;

        // Apply Movement Stats
        if (controller != null)
        {
            controller.moveSpeed = data.moveSpeed;
            controller.jumpForce = data.jumpForce;
        }

        // Apply Health Stats
        if (health != null)
        {
            health.maxHealth = data.maxHealth;
        }

        // Apply Character Sprite
        if (spriteRenderer != null && data.characterIcon != null)
        {
            spriteRenderer.sprite = data.characterIcon;
        }

        // Apply Animation Controller
        if (animator != null)
        {
            if (data.animatorController != null)
            {
                animator.runtimeAnimatorController = data.animatorController;
                Debug.Log(gameObject.name + " applied Animator Controller: " +
                          data.animatorController.name);
            }
            else
            {
                Debug.LogWarning(
                    "Animator Controller is missing in " + data.characterName
                );
            }
        }

        Debug.Log(
            gameObject.name + " applied character: " +
            data.characterName
        );
    }
}

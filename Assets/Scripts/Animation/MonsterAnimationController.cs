
using UnityEngine;

public class MonsterAnimationController : MonoBehaviour
{
    private Animator animator;

    private static readonly int IsWalking =
        Animator.StringToHash("IsWalking");

    private static readonly int Attack =
        Animator.StringToHash("Attack");

    private static readonly int Hit =
        Animator.StringToHash("Hit");

    private static readonly int Die =
        Animator.StringToHash("Die");

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError(
                "Animator is missing on " + gameObject.name
            );
        }
    }

    public void SetWalking(bool walking)
    {
        if (animator == null) return;

        animator.SetBool(IsWalking, walking);
    }

    public void PlayAttack()
    {
        if (animator == null) return;

        animator.SetTrigger(Attack);
    }

    public void PlayHit()
    {
        if (animator == null) return;

        animator.SetTrigger(Hit);
    }

    public void PlayDeath()
    {
        if (animator == null) return;

        animator.SetTrigger(Die);
    }
}


using UnityEngine;

public class BossMeleeAttack : MonoBehaviour
{
    [Header("Melee Settings")]
    public int damage = 20;
    public float attackRange = 2f;

    private Collider2D bossCollider;

    private void Awake()
    {
        bossCollider = GetComponent<Collider2D>();

        if (bossCollider == null)
            bossCollider = GetComponentInChildren<Collider2D>();
    }

    public bool PerformAttack(GameObject target)
    {
        if (target == null || !target.activeInHierarchy)
            return false;

        PlayerHealth playerHealth =
            target.GetComponent<PlayerHealth>();

        if (playerHealth == null || playerHealth.isDead)
            return false;

        Collider2D playerCollider =
            target.GetComponent<Collider2D>();

        if (playerCollider == null)
            playerCollider = target.GetComponentInChildren<Collider2D>();

        if (bossCollider == null || playerCollider == null)
        {
            Debug.LogWarning("Boss or Player Collider is missing!");
            return false;
        }

        // คำนวณระยะห่างระหว่างขอบ Collider ทั้งสอง
        Bounds bossBounds = bossCollider.bounds;
        Bounds playerBounds = playerCollider.bounds;

        float gapX = Mathf.Max(
            0f,
            Mathf.Max(
                bossBounds.min.x - playerBounds.max.x,
                playerBounds.min.x - bossBounds.max.x
            )
        );

        float gapY = Mathf.Max(
            0f,
            Mathf.Max(
                bossBounds.min.y - playerBounds.max.y,
                playerBounds.min.y - bossBounds.max.y
            )
        );

        float distance = Vector2.Distance(
            new Vector2(gapX, gapY),
            Vector2.zero
        );

        if (distance > attackRange)
        {
            Debug.Log("TARGET IS OUT OF MELEE RANGE!");
            return false;
        }

        playerHealth.TakeDamage(damage, transform.position);

        Debug.Log(
            "BOSS MELEE HIT! " +
            target.name +
            " TAKES " + damage + " DAMAGE!"
        );

        return true;
    }
}

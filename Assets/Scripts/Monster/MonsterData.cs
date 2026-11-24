
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewMonsterData",
    menuName = "DarkWood Quest/Monster Data"
)]
public class MonsterData : ScriptableObject
{
    [Header("Monster Information")]
    public string monsterName;

    [Header("Health")]
    public int maxHealth = 50;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Attack")]
    public int attackDamage = 10;
    public float attackRange = 0.8f;
    public float attackCooldown = 1f;
}

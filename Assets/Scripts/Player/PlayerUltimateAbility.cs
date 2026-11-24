
using UnityEngine;
using System.Collections;

public abstract class PlayerUltimateAbility : MonoBehaviour
{
    public abstract UltimateType AbilityType { get; }

    public abstract IEnumerator ExecuteUltimate();
}

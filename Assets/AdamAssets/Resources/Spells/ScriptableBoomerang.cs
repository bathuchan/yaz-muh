using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoomerangSpellID", menuName = "AbilitySystem/New Boomerang Spell")]
public class ScriptableBoomerang : ScriptableAbility
{
    [Header("Projectile Settings")]
    public BoomerangBehaviour prefab;
    public float baseDamage;
    public float speed;
    public float range;
    public float piercing;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SpawnableSpellID", menuName = "AbilitySystem/New Spawnable Spell")]
public class ScriptableSpawnableSpell : ScriptableAbility
{

    [Header("Spawnable Entity Settings")]
    public SpawnableBehaviour prefab;
    public float baseDamage;
    public float lifeSpan;

}

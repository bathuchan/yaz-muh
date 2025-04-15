using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScriptableAbility : ScriptableObject
{
    [Header("General Ability Attributes")]
    public ElementType element;
    public float cooldownSeconds;
    public bool canCrit; 


}




[CreateAssetMenu(fileName ="ProjectileSpellID" , menuName ="AbilitySystem/New Projectile Spell")]
public class ScriptableProjectileSpell : ScriptableAbility
{

    [Header("Projectile Settings")]
    public ProjectileBehaviour prefab;
    public float baseDamage; 
    public float speed;
    public float range; 

}

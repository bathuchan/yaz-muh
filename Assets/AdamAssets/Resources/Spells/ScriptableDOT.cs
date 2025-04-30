using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DamageOverTimeSpellID" , menuName ="AbilitySystem/New DOT Spell")]
public class ScriptableDOTSpell : ScriptableAbility
{
    
    [Header("Damage Over Time Settings")]
    public float damagePerSecond;
    public float damagePeriod;
    public float duration; 



}

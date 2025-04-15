using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;







public class AbilityFactory : MonoBehaviour
{

    public ScriptableProjectileSpell[] projectileSpells;
    public ScriptableDOTSpell[] damageOverTimeSpells;
    public ScriptableSpawnableSpell[] spawnableSpells;


    private void Awake()
    {
        projectileSpells = Resources.LoadAll<ScriptableProjectileSpell>("Spells/ProjectileSpells");
        damageOverTimeSpells = Resources.LoadAll<ScriptableDOTSpell>("Spells/DOTSpells");
        spawnableSpells = Resources.LoadAll<ScriptableSpawnableSpell>("Spells/SpawnableSpells");
        Debug.Log(projectileSpells[0].name);
    }



    public AbstractAbility MakeAbility(string id){

        for(int i = 0; i < projectileSpells.Length; i++)
        {
            ScriptableProjectileSpell spell = projectileSpells[i];
            if (id.Equals(spell.name))
            {
                PrimaryAbilityStats stats = new PrimaryAbilityStats(spell.element, spell.cooldownSeconds, spell.baseDamage, spell.canCrit);
                return new ProjectileSpell(stats, spell.speed, spell.range, spell.prefab);
            }


        }


        for(int i = 0;i < damageOverTimeSpells.Length; i++)
        {
            ScriptableDOTSpell spell = damageOverTimeSpells[i];
            if (id.Equals(spell.name))
            {
                PrimaryAbilityStats stats = new PrimaryAbilityStats(spell.element, spell.damagePeriod , spell.damagePerSecond , spell.canCrit);
                return new DamageOverTimeSpell( stats , spell.duration, spell.damagePerSecond); 
            }
        }


        for (int i = 0; i < spawnableSpells.Length; i++)
        {
            ScriptableSpawnableSpell spell = spawnableSpells[i];
            if (id.Equals(spell.name))
            {
                PrimaryAbilityStats stats = new PrimaryAbilityStats(spell.element , spell.cooldownSeconds , spell.baseDamage , spell.canCrit);
                return new SpawnAbilitySpell(stats, spell.lifeSpan, spell.prefab);

            }
        }

        Debug.Log(string.Format("couldnt found {0} ::: {1}", id , id == projectileSpells[1].name));
        return null;    
    }




}

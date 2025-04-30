using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;







public class AbilityFactory : MonoBehaviour
{
    public ScriptableAbility[] abilities;


    private void Awake()
    {


        abilities = Resources.LoadAll<ScriptableAbility>("Spells/");

    }



    private AbstractAbility initializeAbility(ScriptableAbility ability)
    {

        switch (ability)
        {
            case ScriptableDOTSpell dot:
                PrimaryAbilityStats stat1 = new PrimaryAbilityStats(dot.element, dot.damagePeriod, dot.damagePerSecond, dot.canCrit);
                return new DamageOverTimeSpell(stat1, dot.duration, dot.damagePerSecond);

            case ScriptableSpawnableSpell spwn:
                PrimaryAbilityStats stat2 = new PrimaryAbilityStats(spwn.element, spwn.cooldownSeconds , spwn.baseDamage , spwn.canCrit);
                return new SpawnAbilitySpell(stat2, spwn.lifeSpan, spwn.prefab);


            case ScriptableProjectileSpell p: 
                PrimaryAbilityStats stat3 = new PrimaryAbilityStats(p.element , p.cooldownSeconds , p.baseDamage , p.canCrit);
                return new ProjectileSpell(stat3 , p.speed , p.range , p.prefab);


            case ScriptableBoomerang bm: 
                PrimaryAbilityStats stat4 = new PrimaryAbilityStats(bm.element, bm.cooldownSeconds , bm.baseDamage , bm.canCrit);
                return new BoomerangSpell(stat4, bm.speed, bm.prefab);

            default:
                return null;
                break;



        }
    }


    public AbstractAbility MakeAbility(string id){


        for(int i = 0; i < abilities.Length; i++)
        {
            ScriptableAbility ability = abilities[i];
            if (id.Equals(ability.name))
                return initializeAbility(ability);

        }
        return null;    
    }




}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireBoomerang : BoomerangBehaviour
{


    DamageOverTimeSpell burnSpell;


    private void Start()
    {
        burnSpell = GameObject.FindFirstObjectByType<AbilityFactory>().MakeAbility("Burnt") as DamageOverTimeSpell;
        burnSpell.damage.Initialize(source.stats.damageMultiplier , source.stats.criticalChance , source.stats.criticalChance );

    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.TryGetComponent<Player>(out Player enemy))
        {

            if (enemy == source) return;
            enemy.abilities.AddDamageOverTime(burnSpell);
        }


        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.Get(damage);
            piercing--;
        }


    }








}

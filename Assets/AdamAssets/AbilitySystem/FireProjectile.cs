using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : ProjectileBehaviour
{

    DamageOverTimeSpell burnSpell;
    private void Start()
    {
        burnSpell = GameObject.FindFirstObjectByType<AbilityFactory>().MakeAbility("Burnt") as DamageOverTimeSpell;
        burnSpell.damage.Initialize(source.stats.damageMultiplier, source.stats.criticalChance, source.stats.criticalMultiplier);
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            //Debug.Log(string.Format("LocatedPlayer {0}", player.gameObject.name));
            if (player != source) player.abilities.AddDamageOverTime(burnSpell);
        }

        if (other.TryGetComponent<IDamageable>(out IDamageable enemy))
            if(other.gameObject != source.gameObject) enemy.Get(damage);
           
    }
}

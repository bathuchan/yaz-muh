using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem
{
    List<AbstractAbility> activeAbilities; 
    List<DamageOverTimeSpell> dots;
    List<AbstractAbility> buffs;
    List<Func<bool>> triggerConditions;

    AbilityFactory factory;

    public AbilitySystem(AbilityFactory factory)
    {
        this.factory = factory;
        this.activeAbilities = new List<AbstractAbility>();
        this.dots = new List<DamageOverTimeSpell>();
        this.buffs = new List<AbstractAbility>();
        this.triggerConditions = new List<Func<bool>>
        {
            () => {     return Input.GetMouseButton(0); }, 
            () => {     return Input.GetMouseButton(1); },
            () => {     return Input.GetMouseButton(2); },
        };
    }


    public void AddActiveAbility(string id){ 

        AbstractAbility spell = factory.MakeAbility(id); 
        spell.isTriggered = triggerConditions[activeAbilities.Count] ; 
        activeAbilities.Add(spell); 
    }

    public void AddDamageOverTime(DamageOverTimeSpell dot) { dots.Add(dot); }

    public void UpdateActives(Player player) { if (activeAbilities.Count <= 0) return; for (int i = 0; i < activeAbilities.Count; i++) activeAbilities[i].Update(player); }

    public void UpdateDamageOverTimes(Player player) { if (dots.Count <= 0) return; for (int i = 0; i < dots.Count; i++) dots[i].Update(player); }

    
    public void ClearDamageOverTimes() { if (dots.Count <= 0) return; for (int i = 0; i < dots.Count; i++) if (dots[i].isThrash) { dots[i] = null; dots.RemoveAt(i); } } 



    public void Update(Player player)
    {
        if(player.transform.CompareTag("isOwner")) UpdateActives(player);
        UpdateDamageOverTimes(player);
        ClearDamageOverTimes();

    }





}

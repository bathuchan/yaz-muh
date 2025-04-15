using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageable
{
    public void Get(Damage damage);
}



public class Damage
{
    public ElementType type;
    public int amount;
    public float perEnemyHealth;
    public int baseDamage;

    public float damageMultiplier = 1f;
    public float critMultiplier = 1f;
    public float critChance;

    



    public Damage( ElementType type ,  int baseDamage ,  float baseCritChance , float perEnemyHealth    ){

        this.type = type;
        this.baseDamage = baseDamage;
        this.perEnemyHealth = perEnemyHealth;
        this.critChance = baseCritChance;

        CalculateAmount();        
       
       

    }

    public void Initialize(float damageMultiplier , float critChance , float critMultiplier )
    {
        this.damageMultiplier = damageMultiplier;
        this.critChance += critChance;
        this.critMultiplier = critMultiplier;

    }




    public void DealTo(Player player)
    {

    }

    public void OmnivampTo(Player player) { 
    
    
    }

    public void CalculateAmount() {
        amount =  Mathf.FloorToInt(    
            (IsCrit(this.critChance)) ?  baseDamage * damageMultiplier * critMultiplier : baseDamage * damageMultiplier     
        ) ;
            

    }

    public bool IsCrit(float critChance)
    {
        float clamped =  Mathf.Clamp(critChance, 0f, 100f);
        return (clamped < UnityEngine.Random.Range(0f, 100f));
    } 


    


    
}

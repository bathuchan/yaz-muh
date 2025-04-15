using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class PrimaryAbilityStats
{
    public ElementType Type { get; private set; }
    public float CooldownInSeconds { get; private set; }
    public float BaseDamage { get; private set; }
    public bool CanCrit { get; private set; }


    const float MAX_COOLDOWN_SECS = float.MaxValue;
    const float MAX_DAMAGE = float.MaxValue;


    public PrimaryAbilityStats(ElementType type, float cooldownInSeconds, float baseDamage, bool canCrit)
    {
        Type = type;
        CooldownInSeconds = cooldownInSeconds;
        BaseDamage = baseDamage;
        CanCrit = canCrit;
    }

    public void SetType(ElementType type) {

        if (type == ElementType.NONE)
            Debug.LogWarning("Assigned Element Type is null");
        else if (Type == ElementType.NONE)
            Debug.LogWarning("Element Type is null");
        else { Type = type; }
    
    }

    public void SetCooldown(float cooldownInSeconds)
    {
        if (cooldownInSeconds < 0)
            Debug.LogWarning("Cooldown is negative");
        else
            CooldownInSeconds = Mathf.Clamp(cooldownInSeconds, 0f, MAX_COOLDOWN_SECS);
    }


    public void SetBaseDamage(float damage)
    {
        if (damage < 0)
            Debug.LogWarning("Damage is negative");
        else
            BaseDamage = Mathf.Clamp(damage, 0f, MAX_DAMAGE);
    }

    public void SetCanCrit(bool canCrit) => CanCrit = canCrit;

}


public abstract class  AbstractAbility
{
    public Timer cooldown;
    public PrimaryAbilityStats stats;
    public bool isThrash = false;
    public System.Func<bool> isTriggered = null;
    public static  readonly Timer nullTimer = new Timer(0f , Mathf.Infinity);



    public AbstractAbility(PrimaryAbilityStats stats)
    {
        this.stats = stats;
        this.cooldown = new Timer(Time.time, stats.CooldownInSeconds); 
        this.isTriggered = () => true;
    }

    public AbstractAbility(PrimaryAbilityStats stats , System.Func<bool> isTriggered)
    {
        this.stats = stats ;
        this.cooldown = new Timer(Time.time, stats.CooldownInSeconds);
        this.isTriggered = isTriggered;

    }



    public virtual void CastBy(Player player){ }
    


    public virtual void Update(Player player)
    {
        if (cooldown.IsFinished && isTriggered())
        {
            cooldown.Reset();
            CastBy(player);
        }


    }



}








public class ProjectileSpell : AbstractAbility
{

    public ProjectileBehaviour projectile;
    public float speed;
    public float range;
    public float baseDamage;


    public ProjectileSpell(PrimaryAbilityStats stats ,float speed , float range , ProjectileBehaviour projectile) : base(stats)
    {
        this.speed = speed; 
        this.range = range;
        this.stats = stats;
        this.projectile = projectile;
    }

    public ProjectileSpell(PrimaryAbilityStats stats , float speed, float range, ProjectileBehaviour projectile , System.Func<bool> isTriggered) : 
        base( stats , isTriggered)
    {
        this.stats = stats ;
        this.speed = speed;
        this.range = range;
        this.projectile = projectile;

    }




    public override void CastBy(Player player)
    {

        ProjectileBehaviour projectile = Object.Instantiate(this.projectile, player.transform.position, Quaternion.identity);
        projectile.damage = new Damage(ElementType.NONE, 100 , 0f , 0f);
        projectile.direction = (player.inputSystem.targetPos - player.transform.position).normalized; 
        projectile.speed = speed; 
        projectile.range = range;
        projectile.source = player;
    }


}


public class DamageOverTimeSpell : AbstractAbility
{
    int baseDamage;
    public Damage damage;
    public Timer duration;

    public DamageOverTimeSpell(PrimaryAbilityStats stats, float durationSeconds , float damagePerSecond  ) : base(stats)
    {
        // basedamage / duration = dps 
        baseDamage = Mathf.FloorToInt(damagePerSecond * (durationSeconds * stats.CooldownInSeconds) );
        this.damage = new Damage(stats.Type, baseDamage , 0f , 0f);
        this.duration = new Timer(Time.time , durationSeconds);
    }

    public override void CastBy(Player player)
    {
        if (!duration.IsFinished)
        {
            damage.DealTo(player);
            Debug.Log("Yaniyorum Fuat Abi");
            return;
        }

        Debug.Log("This is over");
        isThrash = true;
           
        


            
    }


}


public class SpawnAbilitySpell : AbstractAbility
{
    public Timer entityLifeTime;
    public float lifeTimeInSeconds;

    SpawnableBehaviour entity;

    public SpawnAbilitySpell(PrimaryAbilityStats stats, float entityLifeTime , SpawnableBehaviour entity) : base(stats)
    {
        this.entityLifeTime = AbstractAbility.nullTimer;
        this.stats = stats;
        this.lifeTimeInSeconds = entityLifeTime;
        this.entity = entity;
    }

    public SpawnAbilitySpell(PrimaryAbilityStats stats, float entityLifeTime, SpawnableBehaviour entity , System.Func<bool> isTriggered ) : base(stats , isTriggered)
    {
        this.entityLifeTime = AbstractAbility.nullTimer; // to display entity lifeTimer
        this.stats = stats;
        this.lifeTimeInSeconds = entityLifeTime;
        this.entity = entity;
        this.isTriggered = isTriggered;
    }

    public override void CastBy(Player player)
    {
        Timer lifeSpan = new Timer(Time.time, lifeTimeInSeconds);
        SpawnableBehaviour spawnable =  Object.Instantiate(entity, player.inputSystem.targetPos, Quaternion.identity);
        spawnable.Initialize(lifeSpan, player);
    }




}







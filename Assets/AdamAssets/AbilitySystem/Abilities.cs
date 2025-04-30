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
    


    public void Update(Player player)
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
        Debug.Log("MySpell");
    }


}


public class BoomerangSpell : AbstractAbility
{
    public float speed;
    public float baseDamage;
    public BoomerangBehaviour projectile;
    public BoomerangSpell(PrimaryAbilityStats stats , float speed , BoomerangBehaviour projectile) : base( stats)
    {
        this.speed = speed;
        this.stats = stats;
        this.projectile = projectile;

    }

    public BoomerangSpell(PrimaryAbilityStats stats, float speed, BoomerangBehaviour projectile , System.Func<bool> isTriggered) : base(stats , isTriggered)
    {
        this.speed = speed;
        this.stats = stats;
        this.projectile=projectile;
        this.isTriggered = isTriggered;

    }


    public override void CastBy(Player player) 
    { 
        BoomerangBehaviour boomerang = Object.Instantiate(this.projectile , player.transform.position , Quaternion.identity);
        boomerang.targetPos = player.inputSystem.targetPos;
        boomerang.damage = new Damage(stats.Type , (int)stats.BaseDamage , 0f , 0f);
        boomerang.speed = speed;
        boomerang.source = player;
        boomerang.piercing = 2;

        Debug.Log(string.Format("Casting the spell :{0}", nameof(BoomerangSpell)));
    
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







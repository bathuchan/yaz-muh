using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerStats
{
    public float health;
    public float armor;
    public float damageMultiplier; 
    public float criticalChance;
    public float criticalMultiplier;
    public float shield; 
    public float shieldMultiplier;
    public float cooldownSpeed;
    public float movementSpeed;
    public float healingMultiplier;
    public float omnivampMultiplier;

    public PlayerStats()
    {

    }




}




public class Player : NetworkBehaviour , IDamageable
{

    public AbilitySystem abilities;
    public PlayerInputSystem inputSystem;
    public PlayerStats stats = new PlayerStats();
    //Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        abilities = new AbilitySystem(GameObject.FindFirstObjectByType<AbilityFactory>());
        inputSystem = new PlayerInputSystem(Camera.main);
        abilities.AddActiveAbility("Fireball");
        abilities.AddActiveAbility("MushroomSpell");
    }




// Update is called once per frame
    void Update()
    {
        inputSystem.CameraToWorldPointUpdate(this);
        abilities.Update(this);
    }

    public void Get(Damage damage)
    {
        damage.DealTo(this);
        Debug.Log("Ouch");
    }
}

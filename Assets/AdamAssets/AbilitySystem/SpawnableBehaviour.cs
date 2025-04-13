using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnableBehaviour : MonoBehaviour
{
    public Player source;
    public Timer LifeTimer {  get; private set; }
    public float spawnTime;

    public void Initialize(Timer lifeSpan , Player source)
    {
        LifeTimer = lifeSpan;        
        this.source = source;
    }

    public virtual void Update()
    {
        if (LifeTimer.IsFinished) Destroy(this.gameObject);
    }

}

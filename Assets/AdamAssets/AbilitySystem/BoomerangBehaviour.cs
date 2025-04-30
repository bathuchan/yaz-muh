using System.Collections;
using System.Collections.Generic;
using UnityEngine;














public class BoomerangBehaviour : MonoBehaviour
{
    public float speed;
    public Vector3 targetPos;
    public Damage damage;
    public Player source;
    public int piercing;
    bool destroyWhenArrived = false;
    public void ReturnToSource()
    { 
        targetPos = source.transform.position;
        destroyWhenArrived = true; 
    }
    public bool IsTargetReached() => Vector3.Distance(transform.position, targetPos) <= 0f;
    public bool IsPiercingOver() => piercing <= 0;


    public void FixedUpdate()
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime); 
        if(IsTargetReached())
        {
            if(destroyWhenArrived) { Destroy(gameObject); return; }
            ReturnToSource();
        }
        else if(IsPiercingOver()) ReturnToSource();
        transform.position = newPos;

    }



}

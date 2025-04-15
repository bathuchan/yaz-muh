using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer 
{
    private float startTime;
    public float interval;


    public float StartTime { get { return startTime; } }

    public bool IsFinished { get { return startTime + interval < Time.time; } }

    public float TimeInSeconds { get { return  (!IsFinished) ? ((startTime + interval) - Time.time) : 0f ; } }


    public Timer(float startTime , float interval)
    {
        this.startTime = startTime;
        this.interval = interval;
    }



    public void ReduceByValue(float timeInSeconds) { startTime += timeInSeconds; }
    public void ReduceByPercentage(float percentage) { startTime += interval * percentage; }

    public void Reset() { startTime = Time.time; }



    
    
}

// EXAMPLE USAGE OF TIMER CLASS 

/*
public class Example : MonoBehaviour
{

    Timer cooldown;

    // Start is called before the first frame update
    void Start()
    {
        cooldown = new Timer(Time.time, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldown.IsFinished)
        {
            Debug.Log("Hello cooldown"); 
            cooldown.Reset();
        }
    }
}
*/
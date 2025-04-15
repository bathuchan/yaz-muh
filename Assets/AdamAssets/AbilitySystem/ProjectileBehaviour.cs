using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public Damage damage;
    public float speed;
    public Vector3 direction;
    public float range;
    public Player source;


    float path;


    // Update is called once per frame
    void Update()
    {
        Vector3 movement = (direction * speed * Time.deltaTime);
        path += movement.magnitude; 
        transform.position += movement;
        if(path >= range) Destroy(this.gameObject);

    }


}

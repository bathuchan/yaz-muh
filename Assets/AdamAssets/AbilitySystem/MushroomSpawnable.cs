using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSpawnable : SpawnableBehaviour
{

    Collider coll;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Mushroom is spawned");
        coll = this.GetComponent<Collider>();
        StartCoroutine(GetTriggerReadyCoroutine());

        
    }


    IEnumerator GetTriggerReadyCoroutine()
    {
        coll.enabled = false;
        yield return new WaitForSeconds(2f);
        coll.enabled = true;
    }


    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }




    private void OnTriggerEnter(Collider other)
    {
        if( IsTrigger(other) )
        {
            Debug.Log(string.Format("Mushroom hit the enemy wizard ({0}). Mushroom is disposing..." , other.gameObject.name) );
            Destroy(gameObject);
        }
    }

    public bool IsTrigger(Collider other) => other.gameObject != source && other.TryGetComponent<Player>(out Player enemy);

    private void OnDestroy()
    {
        Debug.Log("Mushroom is disposed");
    }

}

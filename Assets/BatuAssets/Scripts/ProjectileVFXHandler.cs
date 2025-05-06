using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileVFXHandler : MonoBehaviour
{
    public Transform vfxContainer;
    public GameObject[] vfxToSpawn;
    public GameObject[] impactVfxs;
    public bool useImpactVfxOnDespawn = false;
    public GameObject[] despawnVfxs;
    //private List<GameObject> spawnedVfx = new List<GameObject>();
    private Transform parentTransform;
    private bool isAttached = true;
    int trailCount = 0;

    private void Awake()
    {
        parentTransform = transform;
    }

    // Called to initialize VFX on a projectile from projectile
    public void InitializeVFX()
    {

        foreach (GameObject vfx in vfxToSpawn)
        {
            if (vfx == null) continue;
            GameObject vfxInstance = Instantiate(vfx, parentTransform);
            vfxInstance.transform.localPosition = Vector3.zero;
            vfxInstance.transform.localRotation = Quaternion.identity;
            if(vfxInstance.TryGetComponent<TrailRenderer>(out var trail)) 
            {
                trailCount++;
            }
            //spawnedVfx.Add(vfxInstance);
        }

        isAttached = true;
    }

    private void Update()
    {
        // If VFX have been detached and no children remain, destroy this GameObject
        if (!isAttached && transform.childCount == 0+trailCount)
        {
            Destroy(gameObject);
        }
    }


    // Detaches the VFX Parert from the projectile to keep effects when projectile gets destroyed
    public void DetachVFX()
    {
        if (!isAttached) return;

        transform.SetParent(ContainerController.Instance.GetContainer("VFX"));


        isAttached = false;
    }


    // Called for impact vfx 
    public void ImpactVFX()
    {

        StartCoroutine(SpawnVFX(impactVfxs));

    }
    // Called for despawn vfx 
    public void DespawnVFX()
    {

        StartCoroutine(SpawnVFX(despawnVfxs));

    }


    IEnumerator SpawnVFX(GameObject[] vfxArray)
    {
        foreach (GameObject vfx in vfxArray)
        {
            if (vfx == null) continue;

            GameObject instance = Instantiate(vfx, parentTransform);
            //var ps = instance.GetComponent<ParticleSystem>();
            if (instance.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
            {
                Debug.Log(ps.name + " deteceteeeed");
                Destroy(instance, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Debug.Log(ps.name + " deteceted");
                Destroy(instance, 2f);
            }

        }
        yield return null;

    }





}

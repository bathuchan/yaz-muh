using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CurrentProjectiles :MonoBehaviour
{
    public static CurrentProjectiles Instance { get; private set; }

    public Dictionary<int , GameObject> activeProjectiles = new Dictionary<int, GameObject>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

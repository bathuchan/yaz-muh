using UnityEngine;
using System.Collections.Generic;

public class ContainerController : MonoBehaviour
{
    public static ContainerController Instance { get; private set; }

    private Dictionary<string, Transform> containers = new Dictionary<string, Transform>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        InitializeContainers();
    }

    private void InitializeContainers()
    {
        foreach (ChildContainer container in GetComponentsInChildren<ChildContainer>(true))
        {
            if (!containers.ContainsKey(container.containerName))
            {
                containers.Add(container.containerName, container.transform);
            }
            else
            {
                Debug.LogWarning($"Duplicate container name: {container.containerName}");
            }
        }
    }

    public Transform GetContainer(string name)
    {
        if (containers.TryGetValue(name, out Transform containerTransform))
        {
            return containerTransform;
        }

        Debug.LogWarning($"Container not found: {name}");
        return null;
    }
}

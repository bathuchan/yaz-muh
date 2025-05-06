using System.Collections.Generic;
using UnityEngine;

public static class TouchRegistry
{
    private static Dictionary<int, MonoBehaviour> fingerOwners = new Dictionary<int, MonoBehaviour>();


    public static bool ClaimFinger(int fingerId, MonoBehaviour owner)
    {
        if (!fingerOwners.ContainsKey(fingerId))
        {
            fingerOwners[fingerId] = owner;
            return true;
        }
        return false;
    }

    public static bool IsFingerOwned(int fingerId) => fingerOwners.ContainsKey(fingerId);

    public static bool IsFingerOwnedBy(int fingerId, MonoBehaviour owner)
    {
        return fingerOwners.TryGetValue(fingerId, out var currentOwner) && currentOwner == owner;
    }

    public static void ReleaseFinger(int fingerId, MonoBehaviour owner)
    {
        if (IsFingerOwnedBy(fingerId, owner))
        {
            fingerOwners.Remove(fingerId);
        }
    }

    public static void ResetAll()
    {
        fingerOwners.Clear();
    }
}

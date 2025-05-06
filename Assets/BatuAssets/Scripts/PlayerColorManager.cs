using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerColorManager : NetworkBehaviour
{
    public static PlayerColorManager Instance;

    public ColorProperties[] availableColors;
    
    

    private readonly List<int> usedMaterialIndices = new List<int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public int AssignUniqueMaterialIndex()
    {
        for (int i = 0; i < availableColors.Length; i++)
        {
            if (!usedMaterialIndices.Contains(i))
            {
                usedMaterialIndices.Add(i);
                return i;
            }
        }

        Debug.LogWarning("Ran out of unique materials. Reusing index 0.");
        return 0; // fallback
    }

    public void ReleaseMaterialIndex(int index)
    {
        if (usedMaterialIndices.Contains(index))
            usedMaterialIndices.Remove(index);
    }

    public Material GetMaterial(int index) => availableColors[index].colorMaterial;

    public string GetName(int index) => availableColors[index].colorName;

    [System.Serializable]
    public class ColorProperties
    {
        public string colorName;
        public Material colorMaterial;
    }
}

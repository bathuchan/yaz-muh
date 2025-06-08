using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetUsername : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI a;
    void Start()
    {
        if (PlayerInfo.Instance != null)
        {
            a.SetText(PlayerInfo.Instance.username);        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using LiteNetLib.Utils;
using UnityEngine;

public class MatchmakingHandler : MonoBehaviour
{

    public static MatchmakingHandler Instance { get; private set; }
    // Start is called before the first frame update

    public bool queueResponse = false;
    public bool queueWait = false;
    public bool queueLeave = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);

            return;
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (queueResponse && GeneralServerComm.Instance.dataFlag)
        {

            queueResponse = false;
            GeneralServerComm.Instance.dataFlag = false;
            int ret = GeneralServerComm.Instance.return_code;
            if (ret == (int)CMD_STATUS.RET_SUCCESSFUL)
            {
                SceneLoader.LoadSceneStatic("PlayMenu");
                queueWait = true;
            }
        }

        



        if (queueWait && GeneralServerComm.Instance.dataFlag && !queueLeave)
        {

            GeneralServerComm.Instance.dataFlag = false;
            int ret = GeneralServerComm.Instance.return_code;
            if (ret == (int)CMD_STATUS.INFO_JOIN_GAME)
            {
                queueWait = false;
                SceneLoader.LoadSceneStatic("BatuTest");
            }
        }else if (queueLeave && GeneralServerComm.Instance.dataFlag)
        {

            GeneralServerComm.Instance.dataFlag = false;
            int ret = GeneralServerComm.Instance.return_code;
            if (ret == (int)CMD_STATUS.RET_SUCCESSFUL)
            {
                queueLeave = false;
                SceneLoader.LoadSceneStatic("Dashboard");
            }
        }

        


    }


    public void RequestQueueLogin()
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)CMD_STATUS.CMD_JOIN_QUEUE);
        GeneralServerComm.Instance.dataFlag = false;
        GeneralServerComm.Instance.sendToPeer(writer);
        queueResponse = true;
    }
    
    public void RequestQueueLeave()
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)CMD_STATUS.CMD_LEAVE_QUEUE);
        GeneralServerComm.Instance.dataFlag = false;
        GeneralServerComm.Instance.sendToPeer(writer);
        queueLeave = true;
    }
}

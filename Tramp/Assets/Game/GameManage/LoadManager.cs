using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadManager : NetworkBehaviour
{
    public class MyMsgType
    {
        public static short Load = MsgType.Highest + 1;
    };

    public class LoadMessage : MessageBase
    {
        public bool Loaded;
    }

    AsyncOperation asyncOperation;

    MyNetworkManager netManager;
    bool isClientReady;
    bool isflag;
    // Use this for initialization
    void Start()
    {
        isClientReady = false;
        netManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        isflag = true;

        NetworkServer.RegisterHandler(MyMsgType.Load, OnClientReady);


    }

    // Update is called once per frame
    void Update()
    {
        //if (asyncOperation.progress < 0.9f) return;

        if (MyNetworkManager.discovery.isServer)
        {
            if (isClientReady && isflag == true)
            {
                isflag = false;
                netManager.ServerChangeScene("main");
            }
        }
        else
        {
            StartCoroutine("ClientReady");
            isClientReady = true;
        }


    }

    IEnumerator ClientReady()
    {
        CallClientReady();
        //yield return new WaitForSeconds(1);
        //int n = 100;
        //while (n-- > 0)
        //{
        //    CallClientReady();
        //    yield return new WaitForSeconds(1);
        //}

        yield return null;
    }

    void CallClientReady()
    {
        LoadMessage msg = new LoadMessage();
        msg.Loaded=true;
        MyNetworkManager.networkClient.Send(MyMsgType.Load,msg);
    }

    public void OnClientReady(NetworkMessage netMsg)
    {
        Debug.Log("Call OnClientReadyHandler");
        LoadMessage msg = netMsg.ReadMessage<LoadMessage>();
        isClientReady = msg.Loaded;
    }

    [ClientRpc]
    public void RpcAutoCreatePlayer()
    {
        netManager.autoCreatePlayer = true;
    }

    IEnumerator ClientStart()
    {
        yield return new WaitForSeconds(1);
        asyncOperation.allowSceneActivation = true;
    }

}

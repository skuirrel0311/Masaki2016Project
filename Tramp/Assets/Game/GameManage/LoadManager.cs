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

    }

    AsyncOperation asyncOperation;

    MyNetworkManager netManager;
    bool isClientReady=false;
    bool isflag;

    float Timer = 0;
    // Use this for initialization
    void Start()
    {
        isClientReady = false;
        netManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        isflag = true;
        Timer = 0;
        NetworkServer.RegisterHandler(MyMsgType.Load, OnClientReady);
    }

    // Update is called once per frame
    void Update()
    {
        //サーバーは受信状況を見てからスタート
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
            CallClientReady();
            isClientReady = true;
        }

        Timer += Time.deltaTime;
        if(Timer>45)
        {
            Timer = 0;
            netManager.DiscoveryShutdown();
        }
    }

    void OnDestroy()
    {
        NetworkServer.UnregisterHandler(MyMsgType.Load);
    }

    void CallClientReady()
    {
        MyNetworkManager.networkClient.Send(MyMsgType.Load, new LoadMessage());
    }

    public void OnClientReady(NetworkMessage netMsg)
    {
        Debug.Log("Call OnClientReadyHandler");
        isClientReady = true;
    }
}

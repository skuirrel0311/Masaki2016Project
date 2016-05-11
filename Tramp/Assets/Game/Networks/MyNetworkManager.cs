using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Collections;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{

    MyNetworkDiscovery discovery;
    public string IpAddress;
    SoundManager soundManager;

    public bool isStarted = false;

    // Use this for initialization
    void Start()
    {
        discovery = GetComponent<MyNetworkDiscovery>();
        soundManager = GetComponent<SoundManager>();
        isStarted = false;
    }

    void Update()
    {
        if (IsClientConnected())
        {
            Debug.Log("isConnet");
        }
    }

    int count = 0;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        count++;
        if (count >= 3)
        {
            isStarted = true;
        }
        base.OnServerConnect(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        if (networkSceneName == "main")
        {
            autoCreatePlayer = true;
            if (!GetComponent<NetworkDiscovery>().isServer)
                soundManager.PlayMusic(false);
        }
        else
        {
            autoCreatePlayer = false;
        }
        base.OnClientSceneChanged(conn);
    }

    //ButtonStartHostボタンを押した時に実行
    //IPポートを設定し、ホストとして接続
    public void StartupHost()
    {
        SetPort();
        discovery.Initialize();
        discovery.StartAsServer();
        StartHost();
    }

    //ButtonJoinGameボタンを押した時に実行
    //IPアドレスとポートを設定し、クライアントとして接続
    public void JoinGame()
    {
        SetPort();
        discovery.Initialize();
        discovery.StartAsClient();
    }

    //ポートの設定
    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }
}

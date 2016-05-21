using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Collections;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{

    public static MyNetworkDiscovery discovery;
    public string IpAddress;
    SoundManager soundManager;

    [SerializeField]
    GameObject netmanagerPrefab;

    [SerializeField]
    int PlayerCount = 0;


    float joinTimer;

    public bool isStarted = false;
    bool isJoin;

    // Use this for initialization
    void Start()
    {
        discovery = netmanagerPrefab.GetComponent<MyNetworkDiscovery>();
        soundManager = GetComponent<SoundManager>();
        isStarted = false;
        isJoin = false;
        joinTimer = 0;
    }

    void Update()
    {
        if (PlayerCount >= 3||!discovery.isServer)
        {
            isStarted = true;
        }
        else
        {
            if (PlayerCount == 0) return;
            isStarted = false;
        }
        if (isJoin)
        {
            joinTimer += Time.deltaTime;
            if (joinTimer > 1)
            {
                isJoin = false;
                DiscoveryShutdown();
                Debug.Log("JoinFaild");
            }
        }
    }

    void OnGUI()
    {
        if (isJoin)
        {
            GUI.Label(new Rect(0,0,200,100),"通信中");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (newSceneName == "Menu")
        {
            discovery.isStartClient = false;
            soundManager.GameEnd();
            isStarted = false;
            isJoin = false;
            joinTimer = 0;
        }

        base.ServerChangeScene(newSceneName);
    }


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        PlayerCount++;
        base.OnServerConnect(conn);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        isJoin = false;
        base.OnClientConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();
        DiscoveryShutdown();
        base.OnClientDisconnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        PlayerCount--;
        base.OnServerDisconnect(conn);
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
        if (isJoin) return;
        SetPort();
        discovery.Initialize();
        discovery.StartAsServer();
        StartHost();
    }

    //ButtonJoinGameボタンを押した時に実行
    //IPアドレスとポートを設定し、クライアントとして接続
    public void JoinGame()
    {
        if (isJoin) return;
        SetPort();
        discovery.Initialize();
        discovery.StartAsClient();
        isJoin = true;
        joinTimer = 0;
    }

    public override void OnStopHost()
    {

        networkPort = 0;
        isStarted = false;
        PlayerCount = 0;
        base.OnStopHost();
    }
    public override void OnStopClient()
    {
        networkPort = 0;
        isStarted = false;
        PlayerCount = 0;
        Debug.Log("PortOpenClient");
        base.OnStopClient();
    }

    //ポートの設定
    void SetPort()
    {
        networkPort = 7777;
    }

    public void  DiscoveryShutdown()
    {
        discovery.StopBroadcast();
        NetworkTransport.Shutdown();
        NetworkTransport.Init();
        discovery.isStartClient = false;
    }
}

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Collections;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum Winner
{
    win, lose, draw
}

public delegate void OnJoinFaildHandler();

public class MyNetworkManager : NetworkManager
{

    public static MyNetworkDiscovery discovery;
    public static NetworkClient networkClient;
    public string IpAddress;

    [SerializeField]
    GameObject netmanagerPrefab;

    [SerializeField]
    GameObject SeverPlayerPrefab;

    [SerializeField]
    GameObject ClientPlayerPrefab;

    [SerializeField]
    int PlayerCount
    {
        get { return playercount; }
        set
        {
            playercount = value;
            if (discovery != null)
                discovery.broadcastData = playercount.ToString() + "," + networkPort;
        }
    }
    private int playercount = 0;

    float joinTimer;

    public Winner winner;

    //占領している数
    public int occuping;
    //占領されている数
    public int occupied;

    public bool isStarted = false;
    public bool isJoin;

    public bool PlayerisServer;



    public event OnJoinFaildHandler OnjoinFaild;

    // Use this for initialization
    void Start()
    {
        discovery = netmanagerPrefab.GetComponent<MyNetworkDiscovery>();
        isStarted = false;
        isJoin = false;
        joinTimer = 0;
        winner = Winner.draw;
        PlayerisServer = false;
    }

    void Update()
    {
        if (PlayerCount >= 3 || !discovery.isServer)
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
            if (joinTimer > 5)
            {
                isJoin = false;
                DiscoveryShutdown();
                Debug.Log("JoinFaild");
                GameObject.Find("Panel").GetComponent<Image>().CrossFadeAlpha(0, 0.5f, false);
                if (OnjoinFaild!=null)
                    OnjoinFaild();
            }
        }
    }

    void OnGUI()
    {
        if (isJoin)
        {
            GUI.Label(new Rect(0, 0, 200, 100), "通信中");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (newSceneName == "Menu")
        {
            discovery.isStartClient = false;
            isStarted = false;
            isJoin = false;
            joinTimer = 0;
        }
        base.ServerChangeScene(newSceneName);
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
        if (offlineScene != "Result")
            offlineScene = "Error";
        DiscoveryShutdown();
        base.OnClientDisconnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        PlayerCount--;
        if (offlineScene != "Result")
            offlineScene = "Error";
        if (networkSceneName == "main")
            DiscoveryShutdown();
        base.OnServerDisconnect(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        if (networkSceneName != "main")
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
        playerPrefab = SeverPlayerPrefab;
        DiscoveryShutdown();
        SetPort();
        discovery.Initialize();
        discovery.broadcastData = PlayerCount.ToString() + "," + networkPort;
        discovery.StartAsServer();
        Debug.Log("Start:" + serverBindAddress + ":" + serverBindToIP);
        networkClient = StartHost();
        PlayerisServer = true;
    }

    //ButtonJoinGameボタンを押した時に実行
    //IPアドレスとポートを設定し、クライアントとして接続
    public void JoinGame()
    {
        if (isJoin) return;
        playerPrefab = ClientPlayerPrefab;
        Debug.Log("Join:" + serverBindAddress + "," + serverBindToIP);
        DiscoveryShutdown();
        SetPort();
        discovery.Initialize();
        discovery.StartAsClient();
        isJoin = true;
        joinTimer = 0;
        PlayerisServer = false;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player;
        if (conn.address == "localClient")
        {
            player = (GameObject)GameObject.Instantiate(SeverPlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            player = (GameObject)GameObject.Instantiate(ClientPlayerPrefab, Vector3.zero, Quaternion.identity);

        }
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
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

        networkPort = UnityEngine.Random.Range(7777, 7877);
    }

    public void DiscoveryShutdown()
    {

        if (discovery.isServer)
        {
            StopHost();
        }
        else
        {
            StopClient();
        }

        if (discovery.running)
            discovery.StopBroadcast();
        NetworkTransport.Shutdown();
        NetworkTransport.Init();
        discovery.isStartClient = false;
    }

    public delegate void OnSeverReadeyHandler();

    public event OnSeverReadeyHandler OnServerReadyEvent;

    public override void OnServerReady(NetworkConnection conn)
    {
        if (OnServerReadyEvent != null)
            OnServerReadyEvent();


        base.OnServerReady(conn);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Call Client Error:" + (NetworkConnectionError)errorCode);
        if (offlineScene != "Result")
            offlineScene = "Error";
        DiscoveryShutdown();
        base.OnClientError(conn, errorCode);
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Call Sever Error:" + (NetworkConnectionError)errorCode);
        if (offlineScene != "Result")
            offlineScene = "Error";
        if (networkSceneName == "main")
            DiscoveryShutdown();
        base.OnServerError(conn, errorCode);
    }
}

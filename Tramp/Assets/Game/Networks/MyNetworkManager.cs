﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Collections;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;

public enum Winner
{
    win, lose, draw
}

public class MyNetworkManager : NetworkManager
{

    public static MyNetworkDiscovery discovery;
    public string IpAddress;
    SoundManager soundManager;

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
        winner = Winner.draw;
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
            GUI.Label(new Rect(0, 0, 200, 100), "通信中");
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
        if (networkSceneName == "main")
            DiscoveryShutdown();
        base.OnServerDisconnect(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        if (networkSceneName == "main")
        {
            // if (!GetComponent<NetworkDiscovery>().isServer)
            soundManager.PlayMusic(discovery.isServer);
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
        playerPrefab = SeverPlayerPrefab;
        DiscoveryShutdown();
        SetPort();
        discovery.Initialize();
        discovery.broadcastData = PlayerCount.ToString() + "," + networkPort;
        discovery.StartAsServer();
        Debug.Log("Start:" + serverBindAddress + ":" + serverBindToIP);
        StartHost();
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

        networkPort = Random.Range(7777, 7877);
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

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Call Client Error:" + (NetworkConnectionError)errorCode);
        base.OnClientError(conn, errorCode);
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Call Sever Error:" + (NetworkConnectionError)errorCode);
        base.OnServerError(conn, errorCode);
    }
}

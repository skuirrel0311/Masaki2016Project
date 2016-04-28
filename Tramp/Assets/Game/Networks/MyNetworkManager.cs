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
    GameManager gameManager;

    // Use this for initialization
    void Start()
    {
        discovery = GetComponent<MyNetworkDiscovery>();
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        if (IsClientConnected())
        {
            Debug.Log("isConnet");
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (numPlayers == 1)
        {
            gameManager.PlayMusic();
        }
        base.OnServerAddPlayer(conn, playerControllerId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        if (!GetComponent<NetworkDiscovery>().isServer)
            gameManager.PlayMusic();

        base.OnClientConnect(conn);
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
        SetIPAddress();
        SetPort();
        //NetworkManager.singleton.StartClient();
        discovery.Initialize();
        discovery.StartAsClient();
    }

    void SetIPAddress()
    {
        //Input Fieldに記入されたIPアドレスを取得し、接続する
        // string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        //NetworkManager.singleton.networkAddress = ipAddress;
    }

    //ポートの設定
    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }


    public override void OnStopClient()
    {
        discovery.StopBroadcast();
        discovery.showGUI = true;
    }
}

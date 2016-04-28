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

    // Use this for initialization
    void Start()
    {
        discovery = GetComponent<MyNetworkDiscovery>();
        soundManager = GetComponent<SoundManager>();
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
            soundManager.PlayMusic(true);
        }
        base.OnServerAddPlayer(conn, playerControllerId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        if (!GetComponent<NetworkDiscovery>().isServer)
            soundManager.PlayMusic(false);

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

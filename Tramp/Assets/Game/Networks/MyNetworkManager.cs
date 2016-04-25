using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Collections;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager {

    NetworkDiscovery discovery;
    MatchDesc roomList = null;
    public string IpAddress;
    // Use this for initialization
    void Start () {
        discovery = GetComponent<NetworkDiscovery>();
	}

    void Update()
    {
        if (IsClientConnected())
        {
            Debug.Log("isConnet");
        }
    }

    public override void OnStartHost()
    {

    }

    public override void OnStartClient(NetworkClient client)
    {

    }

    //ButtonStartHostボタンを押した時に実行
    //IPポートを設定し、ホストとして接続
    public void StartupHost()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    //ButtonJoinGameボタンを押した時に実行
    //IPアドレスとポートを設定し、クライアントとして接続
    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    void SetIPAddress()
    {
        //Input Fieldに記入されたIPアドレスを取得し、接続する
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
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

    public override void OnStopHost()
    {
        GetComponent<CNetDicovery>().isStartHost = false;
        base.OnStopHost();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GetComponent<CNetDicovery>().isStartHost = false;
    }
}

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System;

public class MyNetworkDiscovery : NetworkDiscovery {

    MyNetworkManager networkManager;
    public bool isStartClient;
    void Start()
    {
        networkManager = GetComponent<MyNetworkManager>();
        isStartClient = false;
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (networkManager.IsClientConnected()) return;
        if (isStartClient) return;
        isStartClient = true;
        fromAddress = fromAddress.Replace("::ffff:","");
        networkManager.networkAddress = fromAddress;
        networkManager.StartClient();
    }
}

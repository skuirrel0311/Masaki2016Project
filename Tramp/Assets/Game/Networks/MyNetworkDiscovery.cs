using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System;

public class MyNetworkDiscovery : NetworkDiscovery {

    MyNetworkManager networkManager;

    void Start()
    {
        networkManager = GetComponent<MyNetworkManager>();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        networkManager.serverBindToIP = true;
        networkManager.networkAddress = fromAddress;
        networkManager.StartClient();
    }
}

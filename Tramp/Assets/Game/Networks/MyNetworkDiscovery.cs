using UnityEngine;
using UnityEngine.Networking;
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

        networkManager.networkAddress = fromAddress;
        //networkManager.StartClient();
    }
}

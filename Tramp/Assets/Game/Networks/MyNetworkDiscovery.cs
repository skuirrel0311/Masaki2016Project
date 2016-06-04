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
        int intData;
        string[] s = data.Split(',');
        if (!int.TryParse(s[0],out intData) ) return;
        if (intData > 2) return;

        if (!int.TryParse(s[1], out intData)) return;
        networkManager.networkPort = intData;

        isStartClient = true;
        fromAddress = fromAddress.Replace("::ffff:","");
        networkManager.networkAddress = fromAddress;
        networkManager.StartClient();
    }
}

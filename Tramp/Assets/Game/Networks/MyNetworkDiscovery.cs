using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MyNetworkDiscovery : NetworkDiscovery {

    NetworkManager networkManager;

    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        //networkManager.networkAddress = fromAddress;
        NetworkManager.singleton.networkAddress = fromAddress;
        if (NetworkManager.singleton.isNetworkActive) return;
        NetworkManager.singleton.StartClient();
       // networkManager.StartClient();
        //base.OnReceivedBroadcast(fromAddress, data);
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MyNetworkDiscovery : NetworkDiscovery {

    MyNetworkManager networkManager;

    void Start()
    {
        networkManager = GetComponent<MyNetworkManager>();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        //NetworkManager.singleton.networkAddress = fromAddress;
        //if (NetworkManager.singleton.isNetworkActive) return;
        //NetworkManager.singleton.StartClient();
        networkManager.networkAddress = fromAddress;
        networkManager.StartClient();
        //base.OnReceivedBroadcast(fromAddress, data);
    }
}

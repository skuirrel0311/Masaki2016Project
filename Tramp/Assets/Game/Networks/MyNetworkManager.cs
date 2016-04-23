using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MyNetworkManager : NetworkManager {

    NetworkDiscovery discovery;

	// Use this for initialization
	void Start () {
        discovery = GetComponent<NetworkDiscovery>();
	}

    public override void OnStartHost()
    {
        discovery.Initialize();
        discovery.StartAsServer();

    }

    public override void OnStartClient(NetworkClient client)
    {
        discovery.showGUI = false;
    }

    public override void OnStopClient()
    {
        discovery.StopBroadcast();
        discovery.showGUI = true;
    }
}

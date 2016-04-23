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
        discovery.Initialize();
        discovery.StartAsClient();
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

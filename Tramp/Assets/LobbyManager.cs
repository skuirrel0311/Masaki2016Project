using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LobbyManager : MonoBehaviour {

    [SerializeField]
    string NextSceneName;

    [SerializeField]
    GameObject NextButton;

    GameObject networkManager=null;
    MyNetworkManager myNetManager;
    MyNetworkDiscovery myNetDiscoverry;
	// Use this for initialization
	void Start ()
    {

    }

    void Update()
    {
        if (networkManager == null)
        {
            networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
            myNetManager = networkManager.GetComponent<MyNetworkManager>();
            myNetDiscoverry = networkManager.GetComponent<MyNetworkDiscovery>();
        }

        if (myNetManager.isStarted&&myNetDiscoverry.isServer)
        {
            NextButton.SetActive(true);
        }
    }

    public  void OnMoveNextScene()
    {
        myNetManager.ServerChangeScene(NextSceneName); 
    }
}

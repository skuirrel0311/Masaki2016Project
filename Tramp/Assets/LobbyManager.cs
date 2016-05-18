using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class LobbyManager : MonoBehaviour {

    [SerializeField]
    string NextSceneName;

    [SerializeField]
    GameObject NextButton;

    [SerializeField]
    GameObject _2PSprite;

    Text _2pText;


    GameObject networkManager=null;
    MyNetworkManager myNetManager;
    MyNetworkDiscovery myNetDiscoverry;
	// Use this for initialization
	void Start ()
    {
        _2pText = _2PSprite.transform.FindChild("Text").GetComponent<Text>();
    }

    void Update()
    {
        if (networkManager == null)
        {
            networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
            myNetManager = networkManager.GetComponent<MyNetworkManager>();
            myNetDiscoverry = MyNetworkManager.discovery ;
        }

        if (myNetManager.isStarted||!myNetDiscoverry.isServer)
        {
            _2pText.text = "2P接続";
            _2PSprite.GetComponent<Image>().color = Color.white;
        }

        if (!myNetManager.isStarted)
        {
            _2pText.text = "2P未接続";
            _2PSprite.GetComponent<Image>().color = Color.gray;
        }

        if (myNetManager.isStarted&&myNetDiscoverry.isServer)
        {
            NextButton.SetActive(true);
        }
        else
        {
            NextButton.SetActive(false);
        }
    }

    public  void OnMoveNextScene()
    {
        myNetManager.ServerChangeScene(NextSceneName); 
    }

    public void OnDesConnect()
    {

        if (myNetDiscoverry.isServer)
        {
            networkManager.GetComponent<MyNetworkManager>().StopHost();
            networkManager.GetComponent<MyNetworkManager>().StopServer() ;
        }
        else
        {
            networkManager.GetComponent<MyNetworkManager>().StopClient();
        }
        myNetManager.DiscoveryShutdown();
        myNetManager.ServerChangeScene("Menu");
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class LobbyManager : NetworkBehaviour
{

    [SerializeField]
    string NextSceneName;

    [SerializeField]
    GameObject NextButton;

    [SerializeField]
    GameObject _2PSprite;

    Text _2pText;

    GameObject networkManager = null;
    MyNetworkManager myNetManager;
    MyNetworkDiscovery myNetDiscoverry;
    // Use this for initialization
    void Start()
    {
        _2pText = _2PSprite.transform.FindChild("Text").GetComponent<Text>();
    }

    void Update()
    {
        if (networkManager == null)
        {
            networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
            myNetManager = networkManager.GetComponent<MyNetworkManager>();
            myNetDiscoverry = MyNetworkManager.discovery;
        }

        if (myNetManager.isStarted || !myNetDiscoverry.isServer)
        {
            _2pText.text = "2P接続";
            _2PSprite.GetComponent<Image>().color = Color.white;
        }

        if (!myNetManager.isStarted)
        {
            _2pText.text = "2P未接続";
            _2PSprite.GetComponent<Image>().color = Color.gray;
        }

        if (myNetManager.isStarted && myNetDiscoverry.isServer)
        {
            NextButton.SetActive(true);
        }
        else
        {
            NextButton.SetActive(false);
        }
    }

    public void OnMoveNextScene()
    {
        if (NextSceneName == "main")
            RpcAutoCreatePlayer();

        myNetManager.ServerChangeScene(NextSceneName);
    }

    [ClientRpc]
    public void RpcAutoCreatePlayer()
    {
        networkManager.GetComponent<MyNetworkManager>().autoCreatePlayer = true;
    }

    public void OnDesConnect()
    {
        myNetManager.DiscoveryShutdown();
        myNetManager.ServerChangeScene("Menu");
    }
}

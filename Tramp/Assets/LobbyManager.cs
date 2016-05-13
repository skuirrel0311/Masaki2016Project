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
            myNetDiscoverry = networkManager.GetComponent<MyNetworkDiscovery>();
        }

        if (myNetManager.isStarted||!myNetDiscoverry.isServer)
        {
            _2pText.text = "2P接続";
            _2PSprite.GetComponent<Image>().color = Color.white;
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

    public void OnDesConnect()
    {
        MyNetworkDiscovery netMana = networkManager.GetComponent<MyNetworkDiscovery>();
        netMana.StopAllCoroutines();
        netMana.StopBroadcast();
        if (netMana.isServer)
        {
            networkManager.GetComponent<MyNetworkManager>().StopHost();
        }
        else
        {
            networkManager.GetComponent<MyNetworkManager>().StopClient();
        }
        
        Destroy(networkManager);
        SceneManager.LoadScene("Menu");
    }
}

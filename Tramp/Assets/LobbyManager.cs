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

    [SerializeField]
    GameObject _2PPlayerObject;

    Text _2pText;

    GameObject networkManager = null;
    MyNetworkManager myNetManager;
    MyNetworkDiscovery myNetDiscoverry;

    bool isSelect = false;
    [SerializeField]
    Image upButton;

    [SerializeField]
    Image downButton;

    ColorBlock DefaultColor;

    [SerializeField]
    SelectSprites SelectSprite;


    // Use this for initialization
    void Start()
    {
        _2pText = _2PSprite.transform.FindChild("Text").GetComponent<Text>();
        _2PPlayerObject.SetActive(false);
        isSelect = false;

    }

    void Update()
    {
        //部屋選択画面の場合
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
            _2PPlayerObject.SetActive(true);

        }

        if (!myNetManager.isStarted)
        {
            _2pText.text = "2P未接続";
            _2PSprite.GetComponent<Image>().color = Color.gray;
            _2PPlayerObject.SetActive(false);
        }

        if (myNetManager.isStarted && myNetDiscoverry.isServer)
        {
            NextButton.SetActive(true);
            GameManager.ChackButtonSelect(ref isSelect,OnMoveNextScene,OnDesConnect,upButton,downButton,SelectSprite);
        }
        else
        {
            if (GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.A, GamepadInput.GamePadInput.Index.One))
                OnDesConnect();
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

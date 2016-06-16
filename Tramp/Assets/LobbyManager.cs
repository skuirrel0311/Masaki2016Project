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


    // Use this for initialization
    void Start()
    {
        //_2pText = _2PSprite.transform.FindChild("Text").GetComponent<Text>();
        _2PPlayerObject.SetActive(false);
        GameObject.Find("Panel").GetComponent<Image>().CrossFadeAlpha(0, 0.5f, false);
        //SceneManager.LoadSceneAsync("main");
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
            //_2pText.text = "2P接続";
            //_2PSprite.GetComponent<Image>().color = Color.white;
            _2PPlayerObject.SetActive(true);

        }

        if (!myNetManager.isStarted)
        {
            //_2pText.text = "2P未接続";
            //_2PSprite.GetComponent<Image>().color = Color.gray;
            _2PPlayerObject.SetActive(false);
        }

        if (myNetManager.isStarted && myNetDiscoverry.isServer)
        {
            NextButton.SetActive(true);
            if(GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.A, GamepadInput.GamePadInput.Index.One))
            {
                OnMoveNextScene();
            }
        }
        else
        {
            NextButton.SetActive(false);
        }
        if (GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.B, GamepadInput.GamePadInput.Index.One))
            OnDesConnect();

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
        StartCoroutine("SceneBack");
        GameObject.Find("Panel").GetComponent<Image>().CrossFadeAlpha(1,0.5f,false);
    }

    IEnumerator  SceneBack()
    {
        yield return  new WaitForSeconds(0.5f);

        myNetManager.DiscoveryShutdown();
        yield return null;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyText : NetworkBehaviour
{
    GameObject youIsToru;
    GameObject youIsHana;
    GameObject toru;
    GameObject hana;
    GameObject connecting;

    GameObject networkManager = null;
    MyNetworkManager myNetManager;
    MyNetworkDiscovery myNetDiscoverry;

    [SerializeField]
    GameObject clientCharactor;

    void Start()
    {
        youIsToru = transform.FindChild("YouIsToru").gameObject;
        youIsHana = transform.FindChild("YouIsHana").gameObject;
        toru = transform.FindChild("Toru").gameObject;
        hana = transform.FindChild("Hana").gameObject;
        connecting = GameObject.Find("connecting");
        toru.SetActive(true);
    }

    void Update()
    {
        if (networkManager == null) GetNetworkManager();

        hana.SetActive(clientCharactor.activeInHierarchy);

        if (myNetDiscoverry.isServer)
        {
            youIsToru.SetActive(true);
            connecting.SetActive(true);
        }
        else
        {
            youIsHana.SetActive(true);
            connecting.SetActive(false);
        }
    }

    void GetNetworkManager()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        myNetManager = networkManager.GetComponent<MyNetworkManager>();
        myNetDiscoverry = MyNetworkManager.discovery;
    }
}

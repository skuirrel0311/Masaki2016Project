using UnityEngine;
using GamepadInput;
using System.Collections;

public class MainGameManager : MonoBehaviour
{
    public static bool IsPause
    {
        get { return isPause; }
    }

    private static bool isPause;

    [SerializeField]
    private GameObject AppealArea;

    private GameObject networkManager;
    private MyNetworkManager myNetManager;
    private MyNetworkDiscovery myNetDiscovery;

    // Use this for initialization
    void Start()
    {
        isPause = false;
        Time.timeScale = 1.0f;
        //AppealArea = GameObject.Find("AppealArea");
        networkManager= GameObject.FindGameObjectWithTag("NetworkManager");
        myNetManager = networkManager.GetComponent<MyNetworkManager>();
        myNetDiscovery = networkManager.GetComponent<MyNetworkDiscovery>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePadInput.GetButtonDown(GamePadInput.Button.Start, GamePadInput.Index.One))
        {
            if (IsPause)
            {
                Time.timeScale = 1.0f;
                isPause = false;
            }
            else
            {
                Time.timeScale = 0;
                isPause = true;
            }
        }
        if (GamePadInput.GetButtonDown(GamePadInput.Button.A, GamePadInput.Index.One)&&IsPause)
        {
            Time.timeScale = 1.0f;
            isPause = false;

            MyNetworkManager man = networkManager.GetComponent<MyNetworkManager>();
            MyNetworkDiscovery dis = networkManager.GetComponent<MyNetworkDiscovery>();

            if (dis.isServer)
            {
                man.GetComponent<MyNetworkManager>().StopHost();
                man.GetComponent<MyNetworkManager>().StopServer();
            }
            else
            {
                man.GetComponent<MyNetworkManager>().StopClient();
            }
            man.DiscoveryShutdown();
            man.ServerChangeScene("Menu");
        }
        ChackWinner();
    }

    //勝利状況をチェックする
    void ChackWinner()
    {
        if (AppealArea.transform.position.z > 0)
        {
            if (myNetDiscovery.isServer)
            {
                myNetManager.winner = Winner.lose;
            }
            else
            {
                myNetManager.winner = Winner.win;
            }
        }
        else if (AppealArea.transform.position.z < 0)
        {
            if (myNetDiscovery.isServer)
            {
                myNetManager.winner = Winner.win;
            }
            else
            {
                myNetManager.winner = Winner.lose;
            }
        }
        else
        {
            myNetManager.winner = Winner.draw;
        }
    }

    void OunGUI()
    {
        if (!isPause) return;

        GUI.Label(new Rect(0,0,200,100),"Aボタンで戻る");
    }
}

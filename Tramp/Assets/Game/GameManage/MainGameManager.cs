using UnityEngine;
using UnityEngine.Networking;
using GamepadInput;
using System.Collections.Generic;
using System.Collections;

public class MainGameManager : NetworkBehaviour
{
    class MainMsgType
    {
        public static short Start = MsgType.Highest + 2;
    }

    public class StartMessage : MessageBase
    {
    }

    public static bool IsPause
    {
        get { return isPause; }
    }

    private static bool isPause;

    private GameObject networkManager;
    private MyNetworkManager myNetManager;
    private MyNetworkDiscovery myNetDiscovery;
    private SoundManager soundManager;

    [SerializeField]
    private List<AppealAreaState> AppealAreas;

    [SerializeField]
    private GameObject StartEffect;

    public int Occupied
    {
        get { return occupied; }
        set { occupied = value; }
    }
    private int occupied = 0;

    public delegate void OnOccupieding();
    public event OnOccupieding OnOccupiedingHnadler;

    public delegate void OnOccupied();
    public event OnOccupied OnOccupiedHnadler;

    void Awake()
    {
        Debug.Log("main Awake");
    }


    // Use this for initialization
    void Start()
    {
        isPause = false;
        Time.timeScale = 1.0f;
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        myNetManager = networkManager.GetComponent<MyNetworkManager>();
        myNetDiscovery = networkManager.GetComponent<MyNetworkDiscovery>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();

        NetworkServer.RegisterHandler(MainMsgType.Start, OnStart);

        if (!myNetDiscovery.isServer)
            MyNetworkManager.networkClient.Send(MainMsgType.Start, new StartMessage());

        Debug.Log(MyNetworkManager.networkSceneName);
    }

    //IEnumerator SendToStart()
    //{
    //    yield return new WaitForSeconds(3);
    //    MyNetworkManager.networkClient.Send(MainMsgType.Start, new StartMessage());
    //}

    public void OnStart(NetworkMessage msg)
    {
        RpcAddPlayer();
    }

    [ClientRpc]
    public void RpcAddPlayer()
    {
        //if (ClientScene.localPlayers.Count > 0) return;
        ClientScene.AddPlayer(MyNetworkManager.networkClient.connection, 0);
        soundManager.PlayMusic();
        StartEffect.SetActive(true);
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
        if (GamePadInput.GetButtonDown(GamePadInput.Button.A, GamePadInput.Index.One) && IsPause)
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
        int oldOccupieding = myNetManager.occuping;
        int oldOccupied = myNetManager.occupied;
        occupied = 0;
        myNetManager.occupied = 0;
        myNetManager.occuping = 0;
        foreach (AppealAreaState state in AppealAreas)
        {
            if (state.isOccupation)
            {
                if (state.isOccupiers == myNetDiscovery.isServer)
                {
                    occupied++;
                    myNetManager.occuping++;
                }
                else
                {
                    occupied--;
                    myNetManager.occupied++;
                }
            }
        }

        if (oldOccupieding < myNetManager.occuping && OnOccupiedingHnadler != null)
        {
            Debug.Log("Clientでた？");
            OnOccupiedingHnadler();
        }

        if (oldOccupied < myNetManager.occupied && OnOccupiedHnadler != null)
        {
            Debug.Log("Clientでた？");
            OnOccupiedHnadler();
        }


        if (occupied > 0)
            myNetManager.winner = Winner.win;
        else if (occupied == 0)
            myNetManager.winner = Winner.draw;
        else
            myNetManager.winner = Winner.lose;

    }

    void OunGUI()
    {
        if (!isPause) return;

        GUI.Label(new Rect(0, 0, 200, 100), "Aボタンで戻る");
    }

    void OnDestroy()
    {
        NetworkServer.UnregisterHandler(MainMsgType.Start);
    }
}

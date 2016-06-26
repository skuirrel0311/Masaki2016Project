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
        public static short GameEnd = MsgType.Highest + 3;
    }

    public class StartMessage : MessageBase
    {
    }

    public class GameEndMessage : MessageBase
    {
    }

    public static bool IsPause
    {
        get { return isPause; }
    }

    private static bool isPause;

    public static bool isGameStart;

    private GameObject networkManager;
    private MyNetworkManager myNetManager;
    private MyNetworkDiscovery myNetDiscovery;
    private SoundManager soundManager;

    [SerializeField]
    public List<AppealAreaState> AppealAreas;

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
    private bool isStart;

    void Awake()
    {
        Debug.Log("main Awake");
    }

    // Use this for initialization
    void Start()
    {
        isPause = false;
        isGameStart = false;
        Time.timeScale = 1.0f;
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        myNetManager = networkManager.GetComponent<MyNetworkManager>();
        myNetDiscovery = networkManager.GetComponent<MyNetworkDiscovery>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        isStart = false;


        if (!myNetDiscovery.isServer)
        {
            MyNetworkManager.networkClient.Send(MainMsgType.Start, new StartMessage());
        }

        NetworkServer.RegisterHandler(MainMsgType.Start, OnStart);
        NetworkServer.RegisterHandler(MainMsgType.GameEnd, OnGameEnd);

    }


    public void OnStart(NetworkMessage msg)
    {
        RpcAddPlayer();
    }

    [ClientRpc]
    public void RpcAddPlayer()
    {
        if (isStart) return;
        //if (ClientScene.localPlayers.Count > 0) return;
        ClientScene.AddPlayer(MyNetworkManager.networkClient.connection, 0);
        isStart = true;
    }

    public void EndAddPlayer()
    {
        StartCoroutine("GameStart");
        StartEffect.SetActive(true);
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(2.5f);
        soundManager.PlayMusic();
        isGameStart = true;
    }

    void Update()
    {
        if (!isStart)
        {
            MyNetworkManager.networkClient.Send(MainMsgType.Start, new StartMessage());
        }

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

    public void GameEnd()
    {
        myNetManager.offlineScene = "Result";

        if (!myNetManager.PlayerisServer)
        {
            StartCoroutine("gameSet");
        }
    }

    IEnumerator gameSet()
    {
        yield return new WaitForSeconds(3);
        MyNetworkManager.networkClient.Send(MainMsgType.GameEnd, new GameEndMessage());
        yield return null;
    }

    public void OnGameEnd(NetworkMessage msg)
    {
        myNetManager.DiscoveryShutdown();
    }

    void OunGUI()
    {
        if (!isPause) return;

        GUI.Label(new Rect(0, 0, 200, 100), "Aボタンで戻る");
    }

    void OnDestroy()
    {
        NetworkServer.UnregisterHandler(MainMsgType.Start);
        isGameStart = false;
    }

}

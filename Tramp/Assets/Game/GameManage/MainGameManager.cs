using UnityEngine;
using GamepadInput;
using System.Collections.Generic;

public class MainGameManager : MonoBehaviour
{
    public static bool IsPause
    {
        get { return isPause; }
    }

    private static bool isPause;

    private GameObject networkManager;
    private MyNetworkManager myNetManager;
    private MyNetworkDiscovery myNetDiscovery;

    [SerializeField]
    private List<AppealAreaState> AppealAreas;

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

    // Use this for initialization
    void Start()
    {
        isPause = false;
        Time.timeScale = 1.0f;
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
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

        if(oldOccupieding<myNetManager.occuping&&OnOccupiedingHnadler!=null)
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
}

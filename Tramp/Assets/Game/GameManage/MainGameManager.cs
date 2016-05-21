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

    // Use this for initialization
    void Start()
    {
        isPause = false;
        Time.timeScale = 1.0f;
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
            GameObject go = GameObject.FindGameObjectWithTag("NetworkManager");

            MyNetworkManager man = go.GetComponent<MyNetworkManager>();
            MyNetworkDiscovery dis = go.GetComponent<MyNetworkDiscovery>();

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
    }

    void OunGUI()
    {
        if (!isPause) return;

        GUI.Label(new Rect(0,0,200,100),"Aボタンで戻る");
    }
}

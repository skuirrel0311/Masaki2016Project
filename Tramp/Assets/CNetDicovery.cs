using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CNetDicovery : MonoBehaviour
{
    /** NetworkDiscovery*/
    private MyNetworkDiscovery netdisc;
    /** NetworkManager*/
    private MyNetworkManager netman;
    bool isflag;

    // Use this for initialization
    void Start()
    {
        netman = GetComponent<MyNetworkManager>();
        netdisc = GetComponent<MyNetworkDiscovery>();
        isflag = false;
    }

    // Update is called once per frame
    void Update()
    {
        // NetworkManagerが開始していたらGUIを消す
        if (netman.isNetworkActive)
        {
            isflag = true;
        }
        else
        {
            isflag = false;
        }
        // NetworkManagerが開始していない時に処理
        if (isflag == false)
        {
            // NetworkDiscoveryがサーバーとして動作していたら、NetworkManagerをHostで開始する
            if (netdisc.isServer)
            {
                // ホストとして開始
                netman.StartHost();
            }
        }
    }
}

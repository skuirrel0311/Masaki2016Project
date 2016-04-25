using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CNetDicovery : MonoBehaviour
{
    /** NetworkDiscovery*/
    private MyNetworkDiscovery netdisc;
    /** NetworkManager*/
    private MyNetworkManager netman;
    public bool isStartHost;

    // Use this for initialization
    void Start()
    {
        netman = GetComponent<MyNetworkManager>();
        netdisc = GetComponent<MyNetworkDiscovery>();
        isStartHost = false;
    }

    // Update is called once per frame
    void Update()
    {
        // NetworkManagerが開始していない時に処理
        //if (isStartHost == false)
        //{
        //    if (netdisc.isServer)
        //    {
        //        netman.StartHost();
        //    }
        //    if (netman.isNetworkActive)
        //    {
        //        isStartHost = true;
        //    }
        //}

        // NetworkDiscoveryがサーバーとして動作していたら、NetworkManagerをHostで開始する
        //if (netdisc.isServer)
        //{
        //    // ホストとして開始
        //    netdisc.broadcastData = netman.networkPort.ToString();
        //}
    }
}

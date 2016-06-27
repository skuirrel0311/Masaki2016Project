using UnityEngine;
using System.Collections;

public class HowToLoadManager : MonoBehaviour
{

    MyNetworkManager netManager;
    void Start()
    {
        netManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        netManager.ServerChangeScene("HowToPlay");
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{

    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("NetworkManager");
        GameObject.Find("Button1").GetComponent<Button>().onClick.AddListener((go.GetComponent<MyNetworkManager>().StartupHost));
        GameObject.Find("Button2").GetComponent<Button>().onClick.AddListener((go.GetComponent<MyNetworkManager>().JoinGame));
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    GameObject networkManager;


    void Start()
    {
        GameObject go = Instantiate(networkManager);
        GameObject.Find("Button1").GetComponent<Button>().onClick.AddListener((go.GetComponent<MyNetworkManager>().StartupHost));
        GameObject.Find("Button2").GetComponent<Button>().onClick.AddListener((go.GetComponent<MyNetworkManager>().JoinGame));
    }

}

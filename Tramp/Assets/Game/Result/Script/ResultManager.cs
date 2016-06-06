using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultManager : MonoBehaviour
{

    [SerializeField]
    GameObject Win;

    [SerializeField]
    GameObject Lose;

    [SerializeField]
    GameObject Draw;

    [SerializeField]
    GameObject HostPlayer;

    [SerializeField]
    GameObject ClientPlayer;

    GameObject networkManager;

    // Use this for initialization
    void Start()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        Winner winner = networkManager.GetComponent<MyNetworkManager>().winner;

        switch (winner)
        {
            case Winner.win:
                Win.SetActive(true);
                break;
            case Winner.lose:
                Lose.SetActive(true);
                break;
            case Winner.draw:
                Draw.SetActive(true);
                break;
        }
        if (networkManager.GetComponent<MyNetworkDiscovery>().isServer)
        {
            if ((winner == Winner.win || winner == Winner.draw))
                ClientPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose",0);
            else
                HostPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
        }
        else
        {
            if ((winner == Winner.win || winner == Winner.draw))
                HostPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
            else
                ClientPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.Start, GamepadInput.GamePadInput.Index.One))
        {
            MyNetworkManager man = networkManager.GetComponent<MyNetworkManager>();
            MyNetworkDiscovery dis = networkManager.GetComponent<MyNetworkDiscovery>();

            if (dis.isServer)
            {
                man.StopHost();
                man.StopServer();
            }
            else
            {
                man.StopClient();
            }
            man.DiscoveryShutdown();
            man.ServerChangeScene("Menu");
        }
    }
}

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

    [SerializeField]
    Text SeverText;

    [SerializeField]
    Text ClientText;

    GameObject networkManager;

    // Use this for initialization
    void Start()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        Winner winner = networkManager.GetComponent<MyNetworkManager>().winner;

        switch (winner)
        {
            case Winner.win:
            case Winner.lose:
                Win.SetActive(true);
                Lose.SetActive(true);

                bool isServerLose = winner == Winner.lose && MyNetworkManager.discovery.isServer == true;
                bool isClientWin = winner == Winner.win && MyNetworkManager.discovery.isServer == false;

                if (isServerLose||isClientWin)
                {
                    Vector2 tmp = Win.GetComponent<RectTransform>().anchoredPosition;
                    Win.GetComponent<RectTransform>().anchoredPosition = Lose.GetComponent<RectTransform>().anchoredPosition;
                    Lose.GetComponent<RectTransform>().anchoredPosition = tmp;
                }

                break;
            case Winner.draw:
                Draw.SetActive(true);
                break;
        }

        if (networkManager.GetComponent<MyNetworkDiscovery>().isServer)
        {
            SeverText.text = networkManager.GetComponent<MyNetworkManager>().occuping.ToString();
            ClientText.text = networkManager.GetComponent<MyNetworkManager>().occupied.ToString();

            if ((winner == Winner.win || winner == Winner.draw)) 
                ClientPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose",0);
            else
                HostPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
        }
        else
        {
            ClientText.text = networkManager.GetComponent<MyNetworkManager>().occuping.ToString();
            SeverText.text = networkManager.GetComponent<MyNetworkManager>().occupied.ToString();

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
            networkManager.GetComponent<MyNetworkManager>().offlineScene = "Menu";
            networkManager.GetComponent<MyNetworkManager>().ServerChangeScene("Menu");
        }
    }
}

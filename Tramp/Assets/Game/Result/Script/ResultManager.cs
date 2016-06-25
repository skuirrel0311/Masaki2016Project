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

    [SerializeField]
    AudioClip winBGM;
    [SerializeField]
    AudioClip loseBGM;
    AudioSource loopAudioSource;

    // Use this for initialization
    void Start()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        MyNetworkManager mynet = networkManager.GetComponent<MyNetworkManager>();
        Winner winner = mynet.winner;
        loopAudioSource = GetComponent<AudioSource>();

        switch (winner)
        {
            case Winner.win:
                loopAudioSource.clip = winBGM;
                break;
            case Winner.lose:
                Win.SetActive(true);
                Lose.SetActive(true);
                loopAudioSource.clip = loseBGM;

                bool isServerLose = winner == Winner.lose &&mynet.PlayerisServer;
                bool isClientWin = winner == Winner.win && !mynet.PlayerisServer;

                if (isServerLose||isClientWin)
                {
                    Vector2 tmp = Win.GetComponent<RectTransform>().anchoredPosition;
                    Win.GetComponent<RectTransform>().anchoredPosition = Lose.GetComponent<RectTransform>().anchoredPosition;
                    Lose.GetComponent<RectTransform>().anchoredPosition = tmp;
                }

                break;
            case Winner.draw:
                loopAudioSource.clip = loseBGM;
                Draw.SetActive(true);
                break;
        }
        loopAudioSource.Play();

        if (mynet.PlayerisServer)
        {
            SeverText.text = mynet.occuping.ToString();
            ClientText.text = mynet.occupied.ToString();

            if (winner == Winner.win) 
                ClientPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose",0);
            else  if(winner==Winner.lose)
                HostPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
        }
        else
        {
            ClientText.text = mynet.occuping.ToString();
            SeverText.text = mynet.occupied.ToString();

            if (winner == Winner.win)
                HostPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
            else if (winner == Winner.lose)
                ClientPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool inputStart= GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.Start, GamepadInput.GamePadInput.Index.One);
        bool inputA = GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.A, GamepadInput.GamePadInput.Index.One);
        if (inputA||inputStart)
        {
            networkManager.GetComponent<MyNetworkManager>().offlineScene = "Menu";
            networkManager.GetComponent<MyNetworkManager>().ServerChangeScene("Menu");

            loopAudioSource.Stop();
        }
    }
}

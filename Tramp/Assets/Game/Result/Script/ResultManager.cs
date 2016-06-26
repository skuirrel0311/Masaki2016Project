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

    Winner winner;
    MyNetworkManager mynet;

    [SerializeField]
    AudioClip winBGM;
    [SerializeField]
    AudioClip loseBGM;
    AudioSource loopAudioSource;

    // Use this for initialization
    void Start()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        mynet = networkManager.GetComponent<MyNetworkManager>();
        winner = mynet.winner;

        SetSprite();
        SetBGM();

        SetAnimation();

        mynet.DiscoveryShutdown();
    }

    void SetBGM()
    {
        loopAudioSource = GetComponent<AudioSource>();
        if (winner == Winner.win) loopAudioSource.clip = winBGM;
        else loopAudioSource.clip = loseBGM;
        
        loopAudioSource.Play();
    }

    void SetSprite()
    {
        if (winner != Winner.draw)
        {
            Win.SetActive(true);
            Lose.SetActive(true);

            bool isServerLose = winner == Winner.lose && mynet.PlayerisServer;
            bool isClientWin = winner == Winner.win && !mynet.PlayerisServer;

            if (isServerLose || isClientWin)
            {
                //座標を入れ替える
                Vector2 tmp = Win.GetComponent<RectTransform>().anchoredPosition;
                Win.GetComponent<RectTransform>().anchoredPosition = Lose.GetComponent<RectTransform>().anchoredPosition;
                Lose.GetComponent<RectTransform>().anchoredPosition = tmp;
            }
        }
        else
        {
            Draw.SetActive(true);
        }

        GameObject canvas = GameObject.Find("Canvas");

        if(mynet.PlayerisServer)
        {
            canvas.transform.FindChild("YouIsToru").gameObject.SetActive(true);
        }
        else
        {
            canvas.transform.FindChild("YouIsHana").gameObject.SetActive(true);
        }
    }

    void SetAnimation()
    {
        //アニメーションの制御
        if (mynet.PlayerisServer)
        {
            //サーバー
            SeverText.text = mynet.occuping.ToString();
            ClientText.text = mynet.occupied.ToString();

            if (winner == Winner.win)
            {
                ClientPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
            }
            else if (winner == Winner.lose)
            {
                HostPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
                HostPlayer.transform.rotation = Quaternion.identity;
            }
        }
        else
        {
            //クライアント
            ClientText.text = mynet.occuping.ToString();  //とった
            SeverText.text = mynet.occupied.ToString();   //とられた

            if (winner == Winner.win)
            {
                HostPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
                HostPlayer.transform.rotation = Quaternion.identity;
            }
            else if (winner == Winner.lose)
            {
                ClientPlayer.GetComponent<Animator>().CrossFadeInFixedTime("lose", 0);
            }
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

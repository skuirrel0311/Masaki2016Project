using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    AudioSource mainMusic;

    [SerializeField]
    AudioClip HostVoiceClip;

    [SerializeField]
    AudioClip ClientVoiceClip;

    public void PlayMusic(bool isSever)
    {
        mainMusic.Play();
        if (isSever)
        {
            mainMusic.PlayOneShot(HostVoiceClip);
        }
        else
        {
            mainMusic.PlayOneShot(ClientVoiceClip);
        }
        Debug.Log("Play MainMusic");
    }

    // Use this for initialization
    void Start()
    {
        mainMusic = GetComponent<AudioSource>();
        isEnd = false;
        isWin = false;
    }
    bool isEnd = false;
    public static bool isWin;
    // Update is called once per frame
    void Update()
    {
        if (mainMusic.time >= mainMusic.clip.length)
        {
            isEnd = true;

            if (!GamepadInput.GamePad.GetButtonDown(GamepadInput.GamePad.Button.A, GamepadInput.GamePad.Index.One))
            {
                return;
            }

            GameObject go = GameObject.FindGameObjectWithTag("NetworkManager");
            MyNetworkDiscovery dis = go.GetComponent<MyNetworkDiscovery>();
            MyNetworkManager man = go.GetComponent<MyNetworkManager>();
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

    public  void GameEnd()
    {
        isEnd = false;
        isWin = false;
        mainMusic.Stop();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 70;
        style.fontStyle = FontStyle.Bold;
        if (isEnd)
        {
            if (isWin)
                GUI.Label(new Rect(400, 400, 1000, 200), "YOU WIN",style);
            else
                GUI.Label(new Rect(400, 400, 1000, 200), "YOU LOSE",style);
        }

    }

}

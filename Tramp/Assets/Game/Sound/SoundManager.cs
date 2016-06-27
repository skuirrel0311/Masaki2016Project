using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource mainMusic;

    [SerializeField]
    AudioSource Voice_A_Audio;

    [SerializeField]
    AudioSource Voice_B_Audio;

    [SerializeField]
    GameObject GameSetEffect;

    private MyNetworkManager myNetManager;
    private MyNetworkDiscovery myNetDiscovery;

    public static bool isEnd = false;
    public static bool isWin;

    private float timer;

    public void PlayMusic()
    {
        mainMusic.Play();
        timer = 0;

        Voice_A_Audio.Play();

        Voice_B_Audio.Play();

        Debug.Log("Play MainMusic");
    }

    // Use this for initialization
    void Start()
    {
        timer = 0;
        isEnd = false;
        isWin = false;
        GameObject netMana = GameObject.FindGameObjectWithTag("NetworkManager");
        myNetManager = netMana.GetComponent<MyNetworkManager>();
        myNetDiscovery = netMana.GetComponent<MyNetworkDiscovery>();
    }
    

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (myNetDiscovery.isServer)
        {
            if (myNetManager.winner == Winner.lose)
            {
                MuteVoices(true,false);
            }
            else if(myNetManager.winner == Winner.win)
            {
                MuteVoices(false,true);
            }
            else
            {
                MuteVoices(false, false);
            }
        }
        else
        {
            if (myNetManager.winner == Winner.lose)
            {
                MuteVoices(false, true);
            }
            else if (myNetManager.winner == Winner.win)
            {
                MuteVoices(true, false);
            }
            else
            {
                MuteVoices(false, false);
            }
        }

        if (timer >= mainMusic.clip.length)
        {
            if (isEnd) return;
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            GameObject inst = Instantiate(GameSetEffect, Vector3.zero, Quaternion.identity) as GameObject;
            inst.transform.parent = cam.transform;
            inst.transform.localPosition = new Vector3(0, 0, 4);

            GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<MainGameManager>().GameEnd();

            isEnd = true;
        }

    }

    void MuteVoices(bool vo_a,bool vo_b)
    {
        Voice_A_Audio.mute = vo_a;
        Voice_B_Audio.mute = vo_b;
    }

    void OnDestroy()
    {
        GameEnd();
    }

    public void GameEnd()
    {
        isEnd = false;
        isWin = false;
        mainMusic.Stop();
    }

    public int GetRemainingTime()
    {
        return (int)((mainMusic.clip.length - mainMusic.time));
    }
}

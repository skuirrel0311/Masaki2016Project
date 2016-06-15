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

    bool isEnd = false;
    public static bool isWin;

    public void PlayMusic()
    {
        mainMusic.Play();

        Voice_A_Audio.Play();

        Voice_B_Audio.Play();

        Debug.Log("Play MainMusic");
    }

    // Use this for initialization
    void Start()
    {
        isEnd = false;
        isWin = false;
        StartCoroutine("StartPlayMusic");
        GameObject netMana = GameObject.FindGameObjectWithTag("NetworkManager");
        myNetManager = netMana.GetComponent<MyNetworkManager>();
        myNetDiscovery = netMana.GetComponent<MyNetworkDiscovery>();
    }

    IEnumerator StartPlayMusic()
    {
        yield return new WaitForSeconds(3);
        PlayMusic();
        yield return null;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (myNetDiscovery.isServer)
        {
            if (myNetManager.winner == Winner.lose)
            {
                Voice_A_Audio.mute = true;
                Voice_B_Audio.mute = false;
            }
            else
            {
                Voice_A_Audio.mute = false;
                Voice_B_Audio.mute = true;
            }
        }
        else
        {
            if (myNetManager.winner == Winner.lose)
            {
                Voice_A_Audio.mute = false;
                Voice_B_Audio.mute = true;
            }
            else
            {
                Voice_A_Audio.mute = true;
                Voice_B_Audio.mute = false;
            }
        }

        if (mainMusic.time >= mainMusic.clip.length)
        {
            if (isEnd) return;
            StartCoroutine("GameSet");
            isEnd = true;
        }

    }

    IEnumerator GameSet()
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject inst = Instantiate(GameSetEffect,Vector3.zero,Quaternion.identity)as GameObject;
        inst.transform.parent = cam.transform;
        inst.transform.localPosition = new Vector3(0,0,4);
        myNetManager.offlineScene = "Result";
        yield return new WaitForSeconds(1);
        myNetManager.DiscoveryShutdown();
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

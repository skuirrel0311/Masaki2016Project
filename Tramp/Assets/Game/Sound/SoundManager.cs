using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource mainMusic;

    [SerializeField]
    AudioSource VoiceAudio;

    [SerializeField]
    AudioClip HostVoiceClip;

    [SerializeField]
    AudioClip ClientVoiceClip;

    private MyNetworkManager myNetManager;

    public void PlayMusic(bool isSever)
    {
        mainMusic.Play();
        if (isSever)
        {
            VoiceAudio.clip = HostVoiceClip;
            VoiceAudio.Play();
        }
        else
        {
            VoiceAudio.clip =ClientVoiceClip;
            VoiceAudio.Play();
        }
        Debug.Log("Play MainMusic");
    }

    // Use this for initialization
    void Start()
    {
        isEnd = false;
        isWin = false;
        myNetManager = GetComponent<MyNetworkManager>();
    }
    bool isEnd = false;
    public static bool isWin;
    // Update is called once per frame
    void Update()
    {
        if (myNetManager.winner == Winner.lose)
        {
            VoiceAudio.mute = true;
        }
        else
        {
            VoiceAudio.mute = false;
        }

        if (mainMusic.time >= mainMusic.clip.length)
        {
            isEnd = true;

            GameObject go = GameObject.FindGameObjectWithTag("NetworkManager");
            MyNetworkManager man = go.GetComponent<MyNetworkManager>();
            man.ServerChangeScene("Result");
        }

    }

    public  void GameEnd()
    {
        isEnd = false;
        isWin = false;
        mainMusic.Stop();
    }

    public int GetRemainingTime()
    {
        return (int)(mainMusic.clip.length - mainMusic.time);
    }
}

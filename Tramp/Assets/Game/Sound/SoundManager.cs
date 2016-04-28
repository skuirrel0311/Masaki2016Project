using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    AudioSource mainMusic;

    [SerializeField]
    AudioClip  HostVoiceClip;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (mainMusic.time >= mainMusic.clip.length)
        {
            MyNetworkDiscovery netMana = GetComponent<MyNetworkDiscovery>();
            netMana.StopBroadcast();
            if (netMana.isServer)
            {
                GetComponent<MyNetworkManager>().StopServer();
                GetComponent<MyNetworkManager>().StopHost();
            }
            else
            {
                GetComponent<MyNetworkManager>().StopClient();
            }
            SceneManager.LoadScene("Menu");
        }

    }
}

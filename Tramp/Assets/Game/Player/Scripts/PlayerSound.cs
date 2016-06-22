using UnityEngine;
using System.Collections;

public class PlayerSound : MonoBehaviour {

    AudioSource[] audioSources;

    public AudioSource LoopAoudioSource { get { return audioSources[0]; } }
    public AudioSource EnableAudioSource { get { return audioSources[1]; } }
    public AudioSource DisableAudioSource { get { return audioSources[2]; } }

    void Awake()
    {
        audioSources = GetComponents<AudioSource>();
        audioSources[0].loop = true;
    }
}

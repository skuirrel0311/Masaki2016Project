using UnityEngine;
using System.Collections;

public class FlowAudioBridge : MonoBehaviour {

    [SerializeField]
    AudioAnalyzer audioAnalyze;

	// Use this for initialization
	void Start () {
        audioAnalyze = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<AudioAnalyzer>();

        if (audioAnalyze == null)
        {
            audioAnalyze = new AudioAnalyzer();
        }
	}

	void Update ()
    {
        GetComponent<Renderer>().material.SetFloat("volume",audioAnalyze.volume);
	}
}

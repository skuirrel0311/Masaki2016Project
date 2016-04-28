using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
  

    AudioSource mainMusic;

    public void PlayMusic()
    {
        mainMusic.Play();
        Debug.Log("Play MainMusic");
    }

	// Use this for initialization
	void Start () {
        mainMusic = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

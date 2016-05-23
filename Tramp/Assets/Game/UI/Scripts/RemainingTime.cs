﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RemainingTime : MonoBehaviour {

    [SerializeField]
    Sprite[] NumberSprites = new Sprite[10];

    [SerializeField]
    Image MInute;

    [SerializeField]
    Image TenSec;

    [SerializeField]
    Image Second;

    private SoundManager soundManager;
	// Use this for initialization
	void Start ()
    {
        soundManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<SoundManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //取り損ねることがあるので念のため
        if (soundManager == null)
        {
            soundManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<SoundManager>();
            if (soundManager == null) return;
        }

        int time = soundManager.GetRemainingTime();

        if (time < 60)
        {
            MInute.color = Color.red;
            TenSec.color = Color.red;
            Second.color = Color.red;
        }


        MInute.sprite = NumberSprites[(int)time/60];
        TenSec.sprite = NumberSprites[(int)(time%60)/10];
        Second.sprite = NumberSprites[time%10];
	}
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Collections;
using System;

public class RemainingTime : MonoBehaviour
{

    [SerializeField]
    Sprite[] NumberSprites = new Sprite[10];

    [SerializeField]
    Image MInute;

    [SerializeField]
    Image TenSec;

    [SerializeField]
    Image Second;

    private SoundManager soundManager;

    public delegate void OnOneMin();
    public event OnOneMin OnOneMinHandler;

    public delegate void aOnTenSec();
    public event aOnTenSec OnTenSceHandler;

    // Use this for initialization
    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {

        int time = soundManager.GetRemainingTime();

        if (!MainGameManager.isGameStart)
            time = 180;

        if (soundManager.isEnd)
            time = 0;

        if (time < 60 && MInute.color != Color.red)
        {

            MInute.color = Color.red;
            TenSec.color = Color.red;
            Second.color = Color.red;
            //nullでなければ実行
            if (OnOneMinHandler != null)
                OnOneMinHandler();
        }

        MInute.sprite = NumberSprites[(int)time / 60];
        TenSec.sprite = NumberSprites[(int)(time % 60) / 10];
        Second.sprite = NumberSprites[time % 10];
    }

}

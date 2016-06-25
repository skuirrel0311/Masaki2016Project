using UnityEngine;
using UnityEngine.UI;
using GamepadInput;
using UnityEngine.Events;
using System.Collections;
using System;


[System.Serializable]
public struct SelectSprites
{
    public Sprite SelectUpSprite;

    public Sprite NonSelectUpSprite;

    public Sprite SelectDownSprite;

    public Sprite NonSelectDownSprite;
}

[System.Serializable]
public enum TitleState
{
    Title, GameStart, CreataRoom, HowtoPlay
}

public class GameManager : MonoBehaviour
{
    public static TitleState sceneState = TitleState.Title;

    [SerializeField]
    GameObject[] Scenes;

    bool isRoomCreateSelect;


    //切り替え用のImage
    [SerializeField]
    Image createRoomImage;
    [SerializeField]
    Image joinGameImage;

    [SerializeField]
    Image GameStartImage;
    [SerializeField]
    Image HowToImage;

    [SerializeField]
    SelectSprites gamestartSprites;

    [SerializeField]
    SelectSprites sprites;

    [SerializeField]
    GameObject popUp=null;

    MyNetworkManager myNetworkmanager;

    TitleState OldScene;

    [SerializeField]
    AudioClip cancelSE;
    [SerializeField]
    AudioClip decisionSE;
    [SerializeField]
    AudioClip selectSE;
    AudioSource audioSource;

    float oldVecY = 0;
    bool oldSelectFlag = true;

    OnJoinFaildHandler PopAct;
    
    AudioSource loopAudioSource;

    void Awake()
    {
        foreach (GameObject go in Scenes)
        {
            go.SetActive(false);
        }
        GameObject obj = GetScene(sceneState);
        OldScene = sceneState;
        if (sceneState == TitleState.CreataRoom)
        {
            OldScene = TitleState.GameStart;
            obj.GetComponent<TitleScene>().isena = true;
            obj.GetComponent<TitleScene>().ZeroPosition();
        }
        obj.SetActive(true);


    }

    void Start()
    {
        isRoomCreateSelect = true;
        GameObject go = GameObject.FindGameObjectWithTag("NetworkManager");
        myNetworkmanager = go.GetComponent<MyNetworkManager>();
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        loopAudioSource = GetComponent<AudioSource>();
        loopAudioSource.Play();

        if (!myNetworkmanager.isJoin)
            GameObject.Find("Panel").GetComponent<Image>().CrossFadeAlpha(0, 0.5f, false);

        PopAct += () =>
        {
            Debug.Log("call PopUp Event");
            popUp.SetActive(true);
        };

        myNetworkmanager.OnjoinFaild += PopAct;
    }

    void OnDestroy()
    {
        myNetworkmanager.OnjoinFaild -= PopAct;
    }

    void Update()
    {
        if (PopUp.isPopUp) return;
        if (myNetworkmanager.isJoin) return;
        ChackBackScene();
        //タイトルの場合
        if (sceneState == TitleState.Title)
        {
            if (GamePadInput.GetButtonDown(GamePadInput.Button.Start, GamePadInput.Index.One) || GamePadInput.GetButtonDown(GamePadInput.Button.A, GamePadInput.Index.One))
            {
                audioSource.PlayOneShot(decisionSE);
                SetScene(TitleState.GameStart);
            }
        }
        else if (sceneState == TitleState.GameStart)
        {
            ChackButtonSelect(ref isRoomCreateSelect,
                //部屋選択シーンへ
                () =>
                {
                    SetScene(TitleState.CreataRoom);
                },
                //ハウトゥーへ
                () =>
                {
                    SetScene(TitleState.HowtoPlay);
                },
                GameStartImage, HowToImage, gamestartSprites);
        }
        else if (sceneState == TitleState.HowtoPlay)
        {
            if (OldScene == sceneState) return;
            myNetworkmanager.offlineScene = "HowToPlay";
            myNetworkmanager.ServerChangeScene("HowToPlay");
        }
        else if (sceneState == TitleState.CreataRoom)
        {
            ChackButtonSelect(ref isRoomCreateSelect, () => { StartCoroutine("StartHost"); }, () => { StartCoroutine("JoinGame"); }, createRoomImage, joinGameImage, sprites);
        }
    }

    IEnumerator StartHost()
    {
        GameObject.Find("Panel").GetComponent<Image>().CrossFadeAlpha(1, 0.5f, false);
        yield return new WaitForSeconds(0.5f);
        loopAudioSource.Stop();
        myNetworkmanager.StartupHost();
        yield return null;
    }

    IEnumerator JoinGame()
    {
        GameObject.Find("Panel").GetComponent<Image>().CrossFadeAlpha(1, 0.5f, false);
        yield return new WaitForSeconds(0.5f);
        loopAudioSource.Stop();
        myNetworkmanager.JoinGame();
        yield return null;
    }

    public void ChackButtonSelect(ref bool selectFlag, UnityAction upCall, UnityAction downCall, Image upImage, Image downImage, SelectSprites sprites)
    {
        Vector2 vec = GamePadInput.GetAxis(GamePadInput.Axis.LeftStick, GamePadInput.Index.One);

        if (vec.y > 0) selectFlag = true;
        else if (vec.y < 0) selectFlag = false;
        if (oldVecY == 0 && vec.y != 0 && oldSelectFlag != selectFlag) audioSource.PlayOneShot(selectSE);

        oldSelectFlag = selectFlag;
        oldVecY = vec.y;
        if (selectFlag)
        {
            upImage.sprite = sprites.SelectUpSprite;
            downImage.sprite = sprites.NonSelectDownSprite;
        }
        else
        {
            upImage.sprite = sprites.NonSelectUpSprite;
            downImage.sprite = sprites.SelectDownSprite;
        }

        if (GamePadInput.GetButtonDown(GamePadInput.Button.A, GamePadInput.Index.One))
        {
            if (selectFlag)
                upCall.Invoke();
            else
                downCall.Invoke();
            audioSource.PlayOneShot(decisionSE);
        }
    }

    void ChackBackScene()
    {
        if (sceneState == TitleState.Title) return;

        int i = (int)sceneState;
        i--;
        if (sceneState == TitleState.HowtoPlay) i--;
        if (GamePadInput.GetButtonDown(GamePadInput.Button.B, GamePadInput.Index.One))
        {
            SetScene((TitleState)i);
            audioSource.PlayOneShot(cancelSE);
        }
    }

    void SetScene(TitleState state)
    {
        OldScene = sceneState;
        sceneState = state;
        //現在のシーンを閉じる
        GetCurrentScene().SetActive(false);

        //指定されたシーンを呼び出す
        GameObject go = GetScene(state);

        go.SetActive(true);
        go.GetComponent<TitleScene>().StartSlideIn();
    }

    //現在のシーンを返す。なければnullを返す
    GameObject GetCurrentScene()
    {
        foreach (GameObject go in Scenes)
        {
            if (go.activeInHierarchy) return go;
        }

        Debug.Log("null");
        return null;
    }

    GameObject GetScene(TitleState state)
    {
        foreach (GameObject go in Scenes)
        {
            if (go.GetComponent<TitleScene>().SceneState == state) return go;
        }

        return null;
    }

}

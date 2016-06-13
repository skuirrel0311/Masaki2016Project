using UnityEngine;
using UnityEngine.UI;
using GamepadInput;
using UnityEngine.Events;
using System.Collections.Generic;

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
    static TitleState sceneState = TitleState.Title;

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

    MyNetworkManager myNetworkmanager;

    TitleState OldScene;

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
    }

    void Update()
    {
        ChackBackScene();
        //タイトルの場合
        if (sceneState == TitleState.Title)
        {
            if (GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.Start, GamepadInput.GamePadInput.Index.One))
            {
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
                    SetScene(TitleState.CreataRoom);
                },
                GameStartImage, HowToImage, gamestartSprites);
        }
        else if (sceneState == TitleState.HowtoPlay)
        {

        }
        else if (sceneState == TitleState.CreataRoom)
        {
            ChackButtonSelect(ref isRoomCreateSelect, myNetworkmanager.StartupHost, myNetworkmanager.JoinGame, createRoomImage, joinGameImage, sprites);
        }

    }

    public static void ChackButtonSelect(ref bool selectFlag, UnityAction upCall, UnityAction downCall, Image upImage, Image downImage, SelectSprites sprites)
    {
        Vector2 vec = GamePadInput.GetAxis(GamePadInput.Axis.LeftStick, GamePadInput.Index.One);

        if (vec.y > 0) selectFlag = true;
        else if (vec.y < 0) selectFlag = false;


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

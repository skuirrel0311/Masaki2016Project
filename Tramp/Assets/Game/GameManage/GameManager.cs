using UnityEngine;
using UnityEngine.UI;
using GamepadInput;
using UnityEngine.Events;
using GamepadInput;
using System.Collections.Generic;

[System.Serializable]
public struct SelectSprites
{
    public Sprite SelectUpSprite;

    public Sprite NonSelectUpSprite;

    public Sprite SelectDownSprite;

    public Sprite NonSelectDownSprite;
}

public enum TitleState
{
    Title, GameStart, CreataRoom, HowtoPlay
}

public class GameManager : MonoBehaviour
{
    static public TitleState sceneState = TitleState.Title;

    [SerializeField]
    GameObject Title;

    [SerializeField]
    GameObject SelectRoom;

    [SerializeField]
    GameObject GameStart;

    [SerializeField]
    GameObject HowTo;

    bool isRoomCreateSelect;

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

    void Awake()
    {
        UnityAction TitleSet = () =>
        {
            Title.SetActive(true);
            SelectRoom.SetActive(false);
            GameStart.SetActive(false);
            HowTo.SetActive(false);
        };

        switch (sceneState)
        {
            case TitleState.Title:
                TitleSet.Invoke();
                break;
            case TitleState.CreataRoom:
                Title.SetActive(false);
                SelectRoom.SetActive(true);
                GameStart.SetActive(false);
                HowTo.SetActive(false);
                break;
            default:
                TitleSet.Invoke();
                break;
        }
    }

    void Start()
    {
        isRoomCreateSelect = true;
        GameObject go = GameObject.FindGameObjectWithTag("NetworkManager");
        myNetworkmanager = go.GetComponent<MyNetworkManager>();
    }

    void Update()
    {
        //タイトルの場合
        if (sceneState == TitleState.Title)
        {
            if (GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.Start, GamepadInput.GamePadInput.Index.One))
            {
                Title.SetActive(false);
                GameStart.SetActive(true);
                sceneState = TitleState.GameStart;
            }
        }
        else if (sceneState == TitleState.GameStart)
        {
            ChackButtonSelect(ref isRoomCreateSelect,
                //部屋選択シーンへ
                () =>
                {
                    sceneState = TitleState.CreataRoom;
                    GameStart.SetActive(false);
                    SelectRoom.SetActive(true);
                },
                //ハウトゥーへ
                () =>
                {
                    sceneState = TitleState.CreataRoom;
                    GameStart.SetActive(false);
                    HowTo.SetActive(true);
                },
                GameStartImage, HowToImage, gamestartSprites);
        }
        else if (sceneState == TitleState.HowtoPlay)
        {
            if (GamePadInput.GetButtonDown(GamePadInput.Button.B, GamePadInput.Index.One))
            {
                sceneState = TitleState.GameStart;
                GameStart.SetActive(true);
                HowTo.SetActive(false);
            }
        }
        else if (sceneState == TitleState.CreataRoom)
        {
            if (GamePadInput.GetButtonDown(GamePadInput.Button.B, GamePadInput.Index.One))
            {
                sceneState = TitleState.GameStart;
                GameStart.SetActive(true);
                SelectRoom.SetActive(false);
            }
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
}

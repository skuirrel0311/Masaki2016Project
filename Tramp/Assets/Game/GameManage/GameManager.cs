using UnityEngine;
using UnityEngine.UI;
using GamepadInput;
using System.Collections;

public class GameManager : MonoBehaviour
{

    bool isRoomCreateSelect;

    Button CreateRoom;
    Button JoinGame;

    [SerializeField]
    ColorBlock ButtonSelectColor;

    ColorBlock DefaultColor;

    MyNetworkManager myNetworkmanager;

    void Start()
    {
        CreateRoom= GameObject.Find("Button1").GetComponent<Button>();
        JoinGame = GameObject.Find("Button2").GetComponent<Button>();
        isRoomCreateSelect = true;
        GameObject go = GameObject.FindGameObjectWithTag("NetworkManager");
        myNetworkmanager = go.GetComponent<MyNetworkManager>();
        CreateRoom.onClick.AddListener((myNetworkmanager.StartupHost));
        JoinGame.onClick.AddListener((myNetworkmanager.JoinGame));
        DefaultColor = CreateRoom.colors;
    }

    void Update()
    {
        ChackButtonSelect(ref isRoomCreateSelect,CreateRoom,JoinGame,DefaultColor,ButtonSelectColor);
    }

    public static void ChackButtonSelect(ref bool selectFlag,Button upButton,Button downButton,ColorBlock defaultColor,ColorBlock SelectColor)
    {
        Vector2 vec = GamePadInput.GetAxis(GamePadInput.Axis.LeftStick, GamePadInput.Index.One);

        if (vec.y > 0) selectFlag = true;
        else if (vec.y < 0) selectFlag = false;


        if (selectFlag)
        {
            upButton.colors = SelectColor;
            downButton.colors = defaultColor;
        }
        else
        {
            upButton.colors = defaultColor;
            downButton.colors = SelectColor;
        }

        if (GamePadInput.GetButtonDown(GamePadInput.Button.A, GamePadInput.Index.One))
        {
            if (selectFlag)
                upButton.onClick.Invoke();
            else
                downButton.onClick.Invoke();
        }
    }
}

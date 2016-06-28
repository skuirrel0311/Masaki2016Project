using UnityEngine;
using GamepadInput;
using System.Collections.Generic;

public class HowToPlayManager : MonoBehaviour
{
    enum MenuState { HowTo, Flow, AppealArea, Penetrate, Attack, Menu }

    public int selectSceneState;
    public int oldSelectSceneState;

    public int selectMenuState;

    public int oldSelectMenuState;
    
    Vector2 oldInputVec;

    [SerializeField]
    AudioClip cancelSE;
    [SerializeField]
    AudioClip decisionSE;
    [SerializeField]
    AudioClip selectSE;

    AudioSource audioSource;
    AudioSource loopAudioSource;

    List<ItemSprite> MenuItems = new List<ItemSprite>();

    List<MovieOrSprite> descriptionTexture = new List<MovieOrSprite>();

    GameObject networkManager;

    void Start()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");

        oldInputVec = Vector2.zero;
        //シーンはメニューで
        selectSceneState = (int)MenuState.Menu;
        oldSelectSceneState = (int)MenuState.Menu;

        //メニューで選択しているのがHowTo
        selectMenuState = (int)MenuState.HowTo;
        oldSelectMenuState = (int)MenuState.HowTo;

        LoadItem();
        LoadSprite();

        MenuItems[selectMenuState].SetActive(true);

        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
    }

    void LoadItem()
    {
        //読み込み
        MenuItems.Add(new ItemSprite("howto1", "howto1_2"));
        MenuItems.Add(new ItemSprite("howto2", "howto2_2"));
        MenuItems.Add(new ItemSprite("howto3", "howto3_2"));
        MenuItems.Add(new ItemSprite("howto4", "howto4_2"));
        MenuItems.Add(new ItemSprite("howto5", "howto5_2"));
    }

    void LoadSprite()
    {
        GameObject canvas = GameObject.Find("Canvas");
        
        descriptionTexture.Add(canvas.transform.FindChild("HowToTexture1").GetComponent<MovieOrSprite>());
        descriptionTexture.Add(canvas.transform.FindChild("HowToTexture2").GetComponent<MovieOrSprite>());
        descriptionTexture.Add(canvas.transform.FindChild("HowToTexture3").GetComponent<MovieOrSprite>());
        descriptionTexture.Add(canvas.transform.FindChild("HowToTexture4").GetComponent<MovieOrSprite>());
        descriptionTexture.Add(canvas.transform.FindChild("HowToTexture5").GetComponent<MovieOrSprite>());

        foreach (MovieOrSprite item in descriptionTexture) item.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector2 leftStick = GamePadInput.GetAxis(GamePadInput.Axis.LeftStick, GamePadInput.Index.One);
        MenuState select = (MenuState)selectSceneState;

        switch (select)
        {
            case MenuState.Menu:
                MenuScene(leftStick.y);
                break;
            case MenuState.HowTo:
                SpriteSecne();
                break;
            case MenuState.Flow:
                MovieSecne();
                break;
            case MenuState.AppealArea:
                MovieSecne();
                break;
            case MenuState.Penetrate:
                MovieSecne();
                break;
            case MenuState.Attack:
                MovieSecne();
                break;
        }

        if (select != MenuState.Menu) ChackSelectScene(leftStick.x);

        oldInputVec = leftStick;
    }

    void MenuScene(float input)
    {
        if (oldInputVec.y == 0 && input != 0)
        {
            oldSelectMenuState = selectMenuState;
            audioSource.PlayOneShot(selectSE);
            UpdateMenuState(input);
            MenuItems[oldSelectMenuState].SetActive(false);
            MenuItems[selectMenuState].SetActive(true);
        }

        if (GamePadInput.GetButtonDown(GamePadInput.Button.A, GamePadInput.Index.One))
        {
            audioSource.PlayOneShot(decisionSE);
            foreach (ItemSprite item in MenuItems) item.SetVisible(false);
            ChangeScene(selectMenuState);
        }

        if(GamePadInput.GetButtonDown(GamePadInput.Button.B,GamePadInput.Index.One))
        {
            if (GameManager.sceneState == TitleState.Title) return;
            audioSource.PlayOneShot(cancelSE);
            GameManager.sceneState = TitleState.Title;

            networkManager.GetComponent<MyNetworkManager>().offlineScene = "Menu";
            networkManager.GetComponent<MyNetworkManager>().ServerChangeScene("Menu");
        }
    }

    void SpriteSecne()
    {
        if (!descriptionTexture[selectSceneState].gameObject.activeSelf)
        {
            descriptionTexture[selectSceneState].gameObject.SetActive(true);
        }

        if (GamePadInput.GetButtonDown(GamePadInput.Button.B, GamePadInput.Index.One))
        {
            audioSource.PlayOneShot(cancelSE);
            descriptionTexture[selectSceneState].gameObject.SetActive(false);
            ChangeScene((int)MenuState.Menu);
        }
    }

    void MovieSecne()
    {
        if (!descriptionTexture[selectSceneState].gameObject.activeSelf)
        {
            descriptionTexture[selectSceneState].gameObject.SetActive(true);
            descriptionTexture[selectSceneState].Play();
        }

            //Bボタンでメニューに戻る
        if (GamePadInput.GetButtonDown(GamePadInput.Button.B, GamePadInput.Index.One))
        {
            audioSource.PlayOneShot(cancelSE);
            descriptionTexture[selectSceneState].Stop();
            descriptionTexture[selectSceneState].gameObject.SetActive(false);
            ChangeScene((int)MenuState.Menu);
        }
    }

    void UpdateMenuState(float input)
    {
        selectMenuState = input < 0 ? selectMenuState + 1 : selectMenuState - 1;
        selectMenuState = Mathf.Clamp(selectMenuState, 0, (int)MenuState.Attack);
    }
    
    void ChangeScene(int scene)
    {
        selectSceneState = scene;

        if (scene == (int)MenuState.Menu)
        {
            foreach (ItemSprite item in MenuItems) item.SetVisible(true);
        }

    }

    void ChangeSideScene()
    {
        if((MenuState)oldSelectSceneState != MenuState.HowTo) descriptionTexture[oldSelectSceneState].Stop();
        MenuItems[selectMenuState].oldIsSelect = false;
        selectMenuState = selectSceneState;
        MenuItems[selectSceneState].oldIsSelect = true;
        descriptionTexture[oldSelectSceneState].gameObject.SetActive(false);

        descriptionTexture[selectSceneState].gameObject.SetActive(true);
        if ((MenuState)selectSceneState != MenuState.HowTo) descriptionTexture[selectSceneState].Play();
    }

    void ChackSelectScene(float input)
    {
        if (oldInputVec.x == 0 && input != 0)
        {
            oldSelectSceneState = selectSceneState;
            audioSource.PlayOneShot(selectSE);

            selectSceneState = input > 0 ? selectSceneState + 1 : selectSceneState - 1;
            selectSceneState = Mathf.Clamp(selectSceneState, 0, (int)MenuState.Attack);
            if(oldSelectSceneState != selectSceneState) ChangeSideScene();
        }
    }
}

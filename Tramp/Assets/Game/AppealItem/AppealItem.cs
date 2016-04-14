using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

public class AppealItem : MonoBehaviour
{
    //空中の足場のリスト
    List<Transform> scaffoldList = new List<Transform>();
    /// <summary>
    /// アイテムを取得したときのHP
    /// </summary>
    public int FirstHp { get; private set; } 

    void Start()
    {
        //生成されている足場をすべて取得する
        GameObject stage = GameObject.Find("Stage");

        foreach(Transform child in stage.transform)
        {
            if(child.name == "scaffold")
            {
                scaffoldList.Add(child);
            }
        }
        SpawnInRandomPosition();
    }

    void Update()
    {
    }

    /// <summary>
    /// ランダムな足場にスポーンする。
    /// </summary>
    public void SpawnInRandomPosition()
    {
        int num = Random.Range(0, scaffoldList.Count);

        transform.position = scaffoldList[num].position;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        FirstHp = 0;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag != "Player") return;
        //すでにアイテムを所持していた
        if (col.GetComponent<PlayerState>().IsPossessionOfItem) return;

        int playerIndex = col.gameObject.GetComponent<PlayerControl>().playerNum;

        if (GamePad.GetButtonDown(GamePad.Button.LeftShoulder, (GamePad.Index)playerIndex))
        {
            EnterThePlayer(col.gameObject);
        }
    }

    /// <summary>
    /// プレイヤーの子になる
    /// </summary>
    /// <param name="player"></param>
    void EnterThePlayer(GameObject player)
    {
        PlayerState playerState = player.GetComponent<PlayerState>();
        transform.parent = playerState.LeftHand;
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
        playerState.IsPossessionOfItem = true;
        playerState.appealItem = gameObject;
        FirstHp = playerState.Hp;
    }
}

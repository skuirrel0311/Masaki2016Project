using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;

public class AppealItem : MonoBehaviour
{
    //空中の足場のリスト
    List<Transform> scaffoldList = new List<Transform>();

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
        if(Input.GetKeyDown(KeyCode.S))
        {
            SpawnInRandomPosition();
        }

    }

    /// <summary>
    /// ランダムな足場にスポーンする。
    /// </summary>
    public void SpawnInRandomPosition()
    {
        int num = Random.Range(0, scaffoldList.Count);

        transform.position = scaffoldList[num].position;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag != "Player") return;
        
        int playerIndex = col.gameObject.GetComponent<PlayerControl>().playerNum;

        if (GamePad.GetButtonDown(GamePad.Button.LeftShoulder,(GamePad.Index)playerIndex))
        {
            //プレイヤーの子にする。
            transform.parent = col.transform;
            col.gameObject.GetComponent<PlayerState>().GetItem();
        }
    }
}

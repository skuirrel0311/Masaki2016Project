﻿using UnityEngine;
using System.Collections;
using GamepadInput;

public class PlayerCreateAnchor : MonoBehaviour
{

    [SerializeField]
    GameObject InstanceAnchor;

    [SerializeField]
    float UncreateDistance = 3;

    private int playerNum;

    // Use this for initialization
    void Start()
    {
        playerNum = GetComponentInParent<PlayerControl>().playerNum;
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePad.GetButtonDown(GamePad.Button.B, (GamePad.Index)playerNum))
        {

            if (CheckNearAnchor())
            {
                GameObject obj;
                obj = (GameObject)Instantiate(InstanceAnchor, transform.position + transform.forward*2+Vector3.up, transform.rotation);
                obj.GetComponent<CreateFlow>().SetCreatePlayerIndex(1);
            }
        }
    }


    //他のアンカーが近すぎないかチェック
    bool CheckNearAnchor()
    {
        //一番近いアンカーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");
        if (objects.Length <= 0) return true;

        float MinimumDistance = 1000000;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < MinimumDistance)
            {
                MinimumDistance = distance;
            }
        }

        if (MinimumDistance < UncreateDistance)
        {
            return false;
        }
        return true;
       
    }
}

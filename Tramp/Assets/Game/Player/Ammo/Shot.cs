﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Shot : NetworkBehaviour
{

    [SerializeField]
    float speed = 10;

    [SerializeField]
    GameObject HitEffect;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {
        ShotMove();
    }

    void ShotMove()
    {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);       
    }


    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Ammohit");
        if (col.gameObject.tag != "Player")
        {
            if (isServer)
            {
                    //衝突位置
                    CmdHitEffect(transform.position);
            }
            else
            {
                Instantiate(HitEffect, transform.position, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    [Command]
    void CmdHitEffect(Vector3 position)
    {
        GameObject go = Instantiate(HitEffect, position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(go);
    }
}

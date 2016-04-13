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
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        ShotMove();
    }
    [Server]
    void ShotMove()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Ammohit");
        Instantiate(HitEffect,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}

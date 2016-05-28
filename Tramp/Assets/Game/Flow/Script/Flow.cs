﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Flow : NetworkBehaviour{

    [SerializeField]
    private float speed=10;
    [SerializeField]
    bool nonDestroy = false;

    public Vector3 FlowVector
    {
        get { return flowVector; }
        set { flowVector = value; isCalc = true; }
    }
    [SyncVar]
    private Vector3 flowVector;

    private Vector3 PlayerVector;

    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; isCalc = true; }
    }
    [SyncVar]
    private Vector3 targetPosition;

    public GameObject targetAnchor;
    public GameObject startAnchor;

    private bool isCalc = true;

    public bool isDestory;

    List<Rigidbody> bodys = new List<Rigidbody>();

    //この流れがどちらのプレイヤーによって作られているか
    public bool WhichCreatePlayer
    {
        get { return whichCreatePlayer; }
        set { whichCreatePlayer = value; }
    }
    [SyncVar]
    private bool whichCreatePlayer;

    //作ったプレイヤーか？
    public bool isCreatePlayer
    {
        get { return isServer == WhichCreatePlayer; }
    }


    private GameObject appealArea;
    bool isrenderd;

    void Awake()
    {
        CreateFlow.flowEffectCount++;
        gameObject.name = "FlowEffect"+CreateFlow.flowEffectCount;
    }

    void Start()
    {
        isCalc = false;
        isDestory = false;
        isrenderd = true;

        if (!isCreatePlayer)
        {
            GetComponent<Renderer>().enabled = false;
            isrenderd = false;
        }

    }
    void Update()
    {
        // if (!isCalc) return;
        if (targetAnchor == null) GetTargetAnchor();
        if (startAnchor == null)
            GetStartAnchor();

        if (!isCreatePlayer&&isrenderd==true)
        {
            GetComponent<Renderer>().enabled = false;
        }

        transform.localScale = new Vector3(2, flowVector.magnitude * 0.5f+1.0f, 2);
        CapsuleCollider capcol = GetComponent<CapsuleCollider>();
        capcol.height = flowVector.magnitude / (flowVector.magnitude * 0.5f);
        capcol.radius = 0.5f;
        capcol.isTrigger = true;

    }

    void GetTargetAnchor()
    {
        List<GameObject> anchor = new List<GameObject>();
        anchor.AddRange(GameObject.FindGameObjectsWithTag("Anchor"));

        targetAnchor = anchor.Find(n => n.transform.position.Equals(targetPosition));

        if (targetAnchor == null)
            Destroy(gameObject);
    }

    //始点を探す
    void GetStartAnchor()
    {
        List<GameObject> anchor = new List<GameObject>();
        anchor.AddRange(GameObject.FindGameObjectsWithTag("Anchor"));

        if (anchor.Count == 0) return;
        else Debug.Log("showAnchor");

        anchor = anchor.FindAll(n => n.name == "InstanceAnchor(Clone)");

        if (anchor.Count == 0) return;

        //startAnchor = anchor.Find(n => n.GetComponent<AnchorHit>().FlowEffect.name == gameObject.name);

        //if (startAnchor == null) return;
        //else Debug.Log("getAnchor");
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player") return;
        if (!col.gameObject.GetComponent<NetworkBehaviour>().isLocalPlayer) return;
        GetComponent<Renderer>().enabled = true;
        Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
        bodys.Add(body);
        body.useGravity = false;
        body.velocity = transform.up* body.velocity.magnitude;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player") PlayerStay(col);
    }

    void PlayerStay(Collider col)
    {
        PlayerState state = col.gameObject.GetComponent<PlayerState>();

        PlayerVector = targetPosition - (col.transform.position + Vector3.up);
        PlayerVector.Normalize();
        Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
        body.useGravity = false;
        body.AddForce(PlayerVector * Time.deltaTime * speed*50,ForceMode.Acceleration);

        if (nonDestroy) return;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag != "Player") return;
        Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
        body.useGravity = true;
        bodys.Remove(body);
        if (!isLocalPlayer) return;
        if (!isCreatePlayer) isrenderd = false;
    }

    void OnDestroy()
    {
        foreach (Rigidbody body in bodys)
        {
            body.useGravity = true;
        }
    }
}

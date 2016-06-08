﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GamepadInput;

public class PlayerState : NetworkBehaviour
{
    [SerializeField]
    public int maxHp = 10;
    public int Hp { get { return hp; } private set { hp = value; } }
    /// <summary>
    /// 体力
    /// </summary>
    [SerializeField]
    [SyncVar]
    private int hp;

    /// <summary>
    /// 復活にかかる時間
    /// </summary>
    [SerializeField]
    float TimeToReturn = 3;

    public GameObject appealItem;

    /// <summary>
    /// 左手のボーンのTransform
    /// </summary>
    public Transform LeftHand;

    public Animator animator;

    /// <summary>
    /// 生きているか？
    /// </summary>
    public bool IsAlive
    {
        get
        {
            if (hp <= 0) return false;
            if (transform.position.y < -3) return false;
            return true;
        }
    }

    /// <summary>
    /// アピールエリアの所有権を持っているか？
    /// </summary>
    public bool IsAreaOwner;

    private int playerIndex = 1;

    void Awake()
    {
        playerIndex = GetComponent<PlayerControl>().playerNum;
        animator = GetComponentInChildren<Animator>();
        Initialize();
    }

    void Start()
    {
        GameObject map = GameObject.Find("Map");
        if ((!MyNetworkManager.discovery.isServer&&!isLocalPlayer)|| (MyNetworkManager.discovery.isServer && isLocalPlayer))
        {
            map.GetComponent<MapPlayerPosition>().SetHostPlayer(gameObject);
        }
        else
        {
            map.GetComponent<MapPlayerPosition>().SetClientPlayer(gameObject);
        }
    }

    void Update()
    {
        //animator.SetBool("HaveItem", IsPossessionOfItem);
        if (!IsAlive)
        {
            //操作できない
            PlayerControl playerControl = GetComponent<PlayerControl>();

            if (playerControl.enabled)
            {
                playerControl.enabled = false;
                //コルーチンを呼ぶのは1回のみ
                StartCoroutine("IsDead");
            }
        }
    }


    [Client]
    void Initialize()
    {
        hp = maxHp;
        PlayerControl playerControl = GetComponent<PlayerControl>();
        animator.CrossFadeInFixedTime("wait", 0.1f);
        playerControl.SetSratPosition();
        playerControl.enabled = true;
    }

    /// <summary>
    /// 死んだ場合に呼ばれる関数
    /// </summary>
    IEnumerator IsDead()
    {
        //フィーバーゲージ減少
        //GetComponent<FeverGauge>().KilledInPlayer();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        //操作できないようにする。
        GetComponent<PlayerControl>().enabled = false;
        animator.CrossFadeInFixedTime("dead", 0.1f);

        yield return new WaitForSeconds(TimeToReturn);
        //3秒後に復活
        Initialize();
        CmdHpReset();
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
        base.OnStartLocalPlayer();
    }

    public void Damege()
    {
        CmdHpDamage();
    }

    [Command]
    void CmdHpReset()
    {
        hp = maxHp;
    }

    [Command]
    void CmdHpDamage()
    {
        //hpを減らす
        hp = hp <= 0 ? 0 : hp - 1;
    }

    void OnGUI()
    {

        if (!isLocalPlayer) return;
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.fontStyle = FontStyle.Bold;
        GUI.TextArea(new Rect(800, 0, 400, 200), "残HP：" + hp, style);
    }
}

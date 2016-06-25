﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XInputDotNetPure;

public class PlayerState : NetworkBehaviour
{
    [SerializeField]
    public int maxHp = 3;
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

    public bool IsInvincible;

    /// <summary>
    /// アピールエリアの所有権を持っているか？
    /// </summary>
    public bool IsAreaOwner;

    /// <summary>
    /// アピール中
    /// </summary>
    public bool IsAppeal;

    private int playerIndex = 1;

    PlayerControl control;
    CameraLockon lockon;

    public bool ISDead
    {
        get { return IsDead; }
        set { IsDead = value; }
    }

    bool IsDead;

    HpGauge hpGauge;

    [SerializeField]
    GameObject DownEffect = null;

    void Awake()
    {
        playerIndex = GetComponent<PlayerControl>().playerNum;
        animator = GetComponentInChildren<Animator>();
        control = GetComponent<PlayerControl>();
        lockon = GameObject.Find("Camera1").GetComponent<CameraLockon>();
        hpGauge = GetComponent<HpGauge>();
        Initialize();
    }

    void Start()
    {
        GameObject map = GameObject.Find("Map");
        if ((!MyNetworkManager.discovery.isServer && !isLocalPlayer) || (MyNetworkManager.discovery.isServer && isLocalPlayer))
        {
            map.GetComponent<MapPlayerPosition>().SetHostPlayer(gameObject);
        }
        else
        {
            map.GetComponent<MapPlayerPosition>().SetClientPlayer(gameObject);
        }

        IsDead = false;
    }

    void Update()
    {
        if (!IsAlive)
        {
            //エフェクトが表示されていなければ表示
            if (!DownEffect.activeSelf)
            {
                DownEffect.SetActive(true);
            }

            if (!isLocalPlayer) return;
            if (!IsDead)
            {
                IsDead = true;
                //コルーチンを呼ぶのは1回のみ
                KillPlayer();
                StartCoroutine("Dead");
            }
        }
        else
        {
            if (DownEffect.activeSelf)
            {
                DownEffect.SetActive(false);
            }
        }
        hpGauge.HitPointUI(hp);
    }

    void LateUpdate()
    {
        if (!isLocalPlayer) return;
        if (!IsDead) return;
        if (!PlayerControl.ChackCurrentAnimatorName(animator, "dead") && !PlayerControl.ChackCurrentAnimatorName(animator, "deadLoop"))
        {
            Debug.Log("Current Animator");
            animator.Play("dead", 0);
        }
    }

    [Client]
    void Initialize()
    {
        hp = maxHp;
        animator.CrossFadeInFixedTime("wait", 0.1f);
        control.SetSratPosition();
        IsInvincible = false;
    }

    void Restoration()
    {
        GetComponent<PlayerShot>().enabled = true;
        GetComponent<PlayerCreateAnchor>().enabled = true;
        lockon.enabled = true;
        animator.CrossFadeInFixedTime("wait", 0.1f);
        IsDead = false;
        CmdHpReset();
    }

    /// <summary>
    /// 死んだ場合に呼ばれる関数
    /// </summary>
    IEnumerator Dead()
    {
        animator.CrossFadeInFixedTime("dead", 0.1f);
        //操作できないようにする。
        GetComponent<PlayerShot>().enabled = false;
        GetComponent<PlayerShot>().playerName.gameObject.SetActive(false);
        GetComponent<PlayerCreateAnchor>().enabled = false;
        lockon.enabled = true;
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        animator.SetLayerWeight(1, 0);
        //殺したプレイヤーにはご褒美を

        float time = 0;
        while (time < TimeToReturn)
        {
            time += Time.deltaTime;
            if (transform.position.magnitude > control.EndArea) control.OutStage(transform.position);
            yield return null;
        }
        //3秒後に復活
        Restoration();
        hp = maxHp;

        IsInvincible = true;
        yield return new WaitForSeconds(1);
        IsInvincible = false;
    }

    //殺したときに呼ばれる
    void KillPlayer()
    {
        CmdKillGet(isServer);
    }


    [Command]
    void CmdKillGet(bool isKilled)
    {
        Debug.Log("CallStart KillGet");
        //アピールエリア(AppealAreaState)を取得
        List<AppealAreaState> areaList = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<MainGameManager>().AppealAreas;
        //自分が占拠したエリアを取得(殺されたほう)
        List<AppealAreaState> areas=new List<AppealAreaState>();
        for (int i = 0; i < areaList.Count; i++)
        {
            if (!areaList[i].isOccupation || (areaList[i].isOccupation && areaList[i].isOccupiers == isKilled))
            {
                areas.Add(areaList[i]);
            }
        }
        //なかったらreturn
        if (areas.Count == 0) return;

        int randomIndex = Random.Range(0, areas.Count);

        areas[randomIndex].ShareMax();

        //殺したほうの占拠フラグにする
        areas[randomIndex].ChangeOccupiers(!isKilled);
        Debug.Log("CallEnd KillGet:areas"+areas.Count);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
        base.OnStartLocalPlayer();
    }

    public void Damege()
    {
        if (IsInvincible) return;
        if (IsDead) return;
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
}

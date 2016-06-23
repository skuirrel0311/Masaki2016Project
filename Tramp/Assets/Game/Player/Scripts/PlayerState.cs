using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XInputDotNetPure;

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

    void Awake()
    {
        playerIndex = GetComponent<PlayerControl>().playerNum;
        animator = GetComponentInChildren<Animator>();
        control = GetComponent<PlayerControl>();
        lockon = GameObject.Find("Camera1").GetComponent<CameraLockon>();
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
        if (!IsAlive)
        {
            if (!isLocalPlayer) return;
            if (!IsDead)
            {
                IsDead = true;
                //コルーチンを呼ぶのは1回のみ
                KillPlayer();
                StartCoroutine("Dead");
            }
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
        GetComponent<PlayerShot>().playerNameText.enabled = false;
        GetComponent<PlayerCreateAnchor>().enabled = false;
        lockon.enabled = true;
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        //殺したプレイヤーにはご褒美を
        
        float time = 0;
        while(time < TimeToReturn)
        {
            time += Time.deltaTime;
            if(transform.position.magnitude > control.EndArea) control.OutStage(transform.position);
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
        //アピールエリア(AppealAreaState)を取得
        List<AppealAreaState> areaList = GameObject.FindGameObjectWithTag("MainGameManager").GetComponent<MainGameManager>().AppealAreas;
        //自分が占拠したエリアを取得(殺されたほう)
        areaList.Where(area => (area.isOccupation && area.isOccupiers == isKilled) || !area.isOccupation);
        //なかったらreturn
        if (areaList.Count == 0) return;

        int randomIndex = Random.Range(0, areaList.Count - 1);

        areaList[randomIndex].ShareMax();

        //殺したほうの占拠フラグにする
        areaList[randomIndex].ChangeOccupiers(!isKilled);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
        base.OnStartLocalPlayer();
    }

    public void Damege()
    {
        if (IsInvincible) return;
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

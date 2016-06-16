using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
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

    /// <summary>
    /// アピールエリアの所有権を持っているか？
    /// </summary>
    public bool IsAreaOwner;

    private int playerIndex = 1;

    PlayerControl control;

    void Awake()
    {
        playerIndex = GetComponent<PlayerControl>().playerNum;
        animator = GetComponentInChildren<Animator>();
        control = GetComponent<PlayerControl>();
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
                //コルーチンを呼ぶのは1回のみ
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
        control.enabled = true;
    }

    void Restoration()
    {
        GetComponent<PlayerShot>().enabled = true;
        GetComponent<PlayerCreateAnchor>().enabled = true;
        control.enabled = true;
        animator.CrossFadeInFixedTime("wait", 0.1f);

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
        GetComponent<PlayerCreateAnchor>().enabled = false;
        control.enabled = false;
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        float time = 0;
        while(time < TimeToReturn)
        {
            time += Time.deltaTime;
            control.OutStage(transform.position);
            yield return null;
        }
        //3秒後に復活
        Restoration();
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

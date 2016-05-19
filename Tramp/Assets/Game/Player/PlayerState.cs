using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GamepadInput;

public class PlayerState : NetworkBehaviour
{
    [SerializeField]
    int maxHp = 10;
    public int Hp { get { return hp; }private set { hp = value; }}
    [SerializeField][SyncVar]
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
    /// アイテムを所持しているか？
    /// </summary>
    public bool IsPossessionOfItem { get; set; }

    /// <summary>
    /// アピール中か？
    /// </summary>
    public bool IsAppealing{ get; private set; }

    private int playerIndex = 1;

    void Awake()
    {
        playerIndex = GetComponent<PlayerControl>().playerNum;
        animator = GetComponentInChildren<Animator>();
        Initialize();
    }

    void Update()
    {
        //animator.SetBool("HaveItem", IsPossessionOfItem);

        //アイテムを所持していたら
        if(IsPossessionOfItem)
        {
            if (GamePad.GetButtonDown(GamePad.Button.RightShoulder, (GamePad.Index)playerIndex))
            {
                //反転
                IsAppealing = IsAppealing ? false : true;
            }
        }

        if(!IsAlive)
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

        if (Input.GetKeyDown(KeyCode.A)) animator.SetTrigger("Dead");
    }

    void Initialize()
    {
        hp = maxHp;
        //transform.position = Vector3.zero;
        //transform.rotation = Quaternion.Euler(Vector3.zero);
        PlayerControl playerControl = GetComponent<PlayerControl>();
        playerControl.enabled = true;
        IsAppealing = false;
        IsPossessionOfItem = false;
    }

    /// <summary>
    /// 死んだ場合に呼ばれる関数
    /// </summary>
    IEnumerator IsDead()
    {
        //フィーバーゲージ減少
        GetComponent<FeverGauge>().KilledInPlayer();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObj in playerObjects)
        {
            //自分以外のプレイヤーのフィーバーゲージを増加させる
            if (gameObject.Equals(playerObj) == false) playerObj.GetComponent<FeverGauge>().KillPlayer();
        }

        //アイテムを所持していたら
        if(IsPossessionOfItem)
        {
            //親子関係をはずしランダムに再設置。
            appealItem.transform.parent = null;
            appealItem.GetComponent<AppealItem>().SpawnInRandomPosition();
        }

        //操作できないようにする。
        GetComponent<PlayerControl>().enabled = false;
        //animator.SetTrigger("Dead");

        yield return new WaitForSeconds(TimeToReturn);
        //3秒後に復活

        Initialize();
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
        base.OnStartLocalPlayer();
    }

    public void Damege()
    {
        CmdHpDamage();

        if (!IsPossessionOfItem || appealItem == null) return;

        //アイテムを所持していたら
        int firstHp = appealItem.GetComponent<AppealItem>().FirstHp;

        //hpがアイテムを所持したときのhpよりも３小さかったら
        if(hp <= firstHp - 3)
        {
            //親子関係を解除
            appealItem.transform.parent = null;
            //ランダムに再配置
            appealItem.GetComponent<AppealItem>().SpawnInRandomPosition();
            IsPossessionOfItem = false;
            IsAppealing = false;
        }
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
        GUI.TextArea(new Rect(800,0,400,200),"残HP："+hp,style);
    }


}

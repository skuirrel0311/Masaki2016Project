using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    int maxHp = 10;
    public int Hp { get; private set; }

    /// <summary>
    /// 復活にかかる時間
    /// </summary>
    [SerializeField]
    float TimeToReturn = 3;
    
    /// <summary>
    /// 初期位置
    /// </summary>
    [SerializeField]
    Transform startPosition;

    public GameObject appealItem;

    /// <summary>
    /// 左手のボーンのTransform
    /// </summary>
    public Transform LeftHand;

    Animator animator;

    /// <summary>
    /// 生きているか？
    /// </summary>
    public bool IsAlive
    {
        get
        {
            if (Hp <= 0) return false;
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

    void Start()
    {
        if(startPosition == null)
        {
            startPosition = GameObject.Find("sartPosition" + GetComponent<PlayerControl>().playerNum).transform;
        }
        animator = GetComponentInChildren<Animator>();
        Initialize();
    }

    void Update()
    {
        animator.SetBool("HaveItem", IsPossessionOfItem);

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
    }

    void Initialize()
    {
        Hp = maxHp;
        if (startPosition == null) startPosition.position = Vector3.zero;
        transform.position = startPosition.position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        PlayerControl playerControl = GetComponent<PlayerControl>();
        playerControl.enabled = true;

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
        //操作できないようにする。
        GetComponent<PlayerControl>().enabled = false;

        yield return new WaitForSeconds(TimeToReturn);
        //3秒後に復活

        Initialize();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ammo") return;
        if (!IsAlive) return;

        Damege();
    }

    void Damege()
    {
        //hpを減らす
        Hp = Hp <= 0 ? 0 : Hp - 1;
        
        if (!IsPossessionOfItem || appealItem == null) return;

        //アイテムを所持していたら
        int firstHp = appealItem.GetComponent<AppealItem>().FirstHp;

        //hpがアイテムを所持したときのhpよりも３小さかったら
        if(Hp <= firstHp - 3)
        {
            //親子関係を解除
            appealItem.transform.parent = null;
            //ランダムに再配置
            appealItem.GetComponent<AppealItem>().SpawnInRandomPosition();
            IsPossessionOfItem = false;
        }

    }
}

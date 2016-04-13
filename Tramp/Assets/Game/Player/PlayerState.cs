using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    int maxHp = 10;
    int hp;

    /// <summary>
    /// 復活にかかる時間
    /// </summary>
    [SerializeField]
    float TimeToReturn = 3;
    
    [SerializeField]
    Vector3 startPosition = Vector3.zero;

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
    public bool IsPossessionOfItem { get; private set; }

    /// <summary>
    /// アピール中か？
    /// </summary>
    public bool IsAppealing{ get; private set; }

    void Start()
    {
        
        Initialize();
    }

    void Update()
    {
        if(!IsAlive)
        {
            //操作できない
            PlayerControl playerControl = GetComponent<PlayerControl>();

            if (playerControl.enabled)
            {
                playerControl.enabled = false;
                StartCoroutine("IsDead");
            }
        }
    }

    void Initialize()
    {
        hp = maxHp;

        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero); 
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
            //自分以外のプレイヤーを
            if (gameObject.Equals(playerObj) == false) playerObj.GetComponent<FeverGauge>().KillPlayer();
        }

        yield return new WaitForSeconds(TimeToReturn);

        //3秒後に復活
        transform.position = startPosition;
        //操作できない
        PlayerControl playerControl = GetComponent<PlayerControl>();
        playerControl.enabled = true;
        Initialize();
    }

    public void GetItem()
    {
        IsPossessionOfItem = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ammo") return;
        if (!IsAlive) return;

        hp = hp <= 0 ? 0 : hp - 1;
    }
}

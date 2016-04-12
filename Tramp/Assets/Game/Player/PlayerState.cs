using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    int maxHp = 10;
    int hp;

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

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if(!IsAlive)
        {
            StartCoroutine("IsDead");
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
        transform.position = startPosition;
        PlayerControl playerControl = GetComponent<PlayerControl>();
        playerControl.enabled = false;
        yield return new WaitForSeconds(3);
        //3秒後に復活
        playerControl.enabled = true;
        Initialize();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ammo") return;
        if (!IsAlive) return;

        hp = hp <= 0 ? 0 : hp - 1;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class AppealAreaState : MonoBehaviour
{
    ///<summary>
    ///乗っているプレイヤー(いないときはnull)
    ///</summary>
    public List<GameObject> RidingPlayer { get { return ridingPlayer; } }
    List<GameObject> ridingPlayer = new List<GameObject>();

    ///<summary>
    ///触れているアンカー
    ///</summary>
    public List<GameObject> OnAnchorList { get { return onAnchorList; } }
    List<GameObject> onAnchorList = new List<GameObject>();

    /// <summary>
    /// 現在乗っている流れ
    /// </summary>
    public GameObject flowObj = null;

    ///<summary>
    ///プレイヤーに乗られているか？
    ///</summary>
    public bool IsRidden { get { return RidingPlayer.Count != 0; } }

    ///<summary>
    ///流れているか？
    ///</summary>
    public bool IsFlowing;

    /// <summary>
    /// アピールエリアの所有者
    /// </summary>
    public GameObject Owner = null;

    /// <summary>
    /// サーバー側が勝ったか？
    /// </summary>
    public Winner ServerWiner;

    //耐久値
    int maxHp = 10;
    public int hp;

    void Start()
    {
        IsFlowing = false;
        hp = maxHp;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
        hp = maxHp;
    }

    void Update()
    {
        if (flowObj == null)
        {
            flowObj = null;
            IsFlowing = false;
        }

        if (Owner != null && !IsFlowing && !IsRidden)
        {
            Owner.GetComponent<PlayerState>().IsAreaOwner = false;
            Owner = null;
        }

        if(transform.position.z < 0)
        {
            ServerWiner = Winner.win;
            return;
        }
        if(transform.position.z == 0)
        {
            ServerWiner = Winner.draw;
            return;
        }
        if(transform.position.z > 0)
        {
            ServerWiner = Winner.lose;
            return;
        }

    }

    //プレイヤーが乗った
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ammo") AmmoHit();
        if (col.gameObject.tag == "Anchor") AnchorHit(col.gameObject);
        if (col.gameObject.name == "Goal") GoalHit();
        if (col.tag != "Player") return;
        if (ridingPlayer.Find(n => n.Equals(col.gameObject)) != null) return;
        //リストにいなかったら追加
        ridingPlayer.Add(col.gameObject);
    }
    //プレイヤーが降りた
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag != "Player") return;
        ridingPlayer.Remove(col.gameObject);
    }
    //流れに乗っているときにアンカーに触れた
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ammo") AmmoHit();
        if (col.gameObject.tag == "Anchor") AnchorHit(col.gameObject);
        if (col.gameObject.name == "Goal") GoalHit();
    }
    //流れていないときにアンカーに触れている
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag != "Anchor") return;

        //既に追加されてたら追加しない
        foreach (GameObject anchor in OnAnchorList)
        {
            if (col.gameObject.Equals(anchor)) return;
        }
        OnAnchorList.Add(col.gameObject);
    }
    //アンカーに触れていたが流れて離れた
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag != "Anchor") return;

        OnAnchorList.Remove(col.gameObject);
    }

    void AmmoHit()
    {
        if (Owner == null) return;
        //所有者がいたら
        hp = --hp > 0 ? hp-- : 0;

        if(hp == 0)
        {
            Owner.GetComponent<PlayerState>().IsAreaOwner = false;
            Owner = null;
            IsFlowing = false;
            Destroy(flowObj);
        }

    }

    void AnchorHit(GameObject col)
    {
        if (flowObj != null && col.Equals(flowObj.GetComponent<Flow>().targetAnchor))
        {
            Destroy(flowObj);
            IsFlowing = false;
            flowObj = null;
        }
    }

    void GoalHit()
    {
        if (transform.position.z < 0)
        {
            ServerWiner = Winner.lose;
        }
        if (transform.position.z > 0)
        {
            ServerWiner = Winner.win;
        }
        if (transform.position.z == 0)
        {
            ServerWiner = Winner.draw;
        }

        GameObject go = GameObject.FindGameObjectWithTag("NetworkManager");
        MyNetworkManager man = go.GetComponent<MyNetworkManager>();
        man.ServerChangeScene("Result");
    }
}

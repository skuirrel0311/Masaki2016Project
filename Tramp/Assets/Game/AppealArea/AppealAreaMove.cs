using UnityEngine;
using System.Collections.Generic;

/*
    ★アピールエリアの動き★
    流れ×：プレイヤー×　＝　中心にもどる
    流れ○：プレイヤー×　＝　流れの終点まで流れる
    流れ×：プレイヤー○　＝　動かない
*/

public class AppealAreaMove : MonoBehaviour
{
    Vector3 oldPosition;
    //移動量
    public Vector3 movement;

    /// <summary>
    /// 乗っているプレイヤー(いないときはnull)
    /// </summary>
    List<GameObject> ridingPlayer = new List<GameObject>();

    /// <summary>
    /// 流れているか？
    /// </summary>
    public bool IsFlowing;
    /// <summary>
    /// プレイヤーに乗られているか？
    /// </summary>
    public bool IsRidden { get { return isRidden; } }
    private bool isRidden;

    /// <summary>
    /// 現在乗っている流れ
    /// </summary>
    public GameObject flowObj = null;

    /// <summary>
    /// 触れているアンカー
    /// </summary>
    public List<GameObject> OnAnchorList = new List<GameObject>();

    void Start()
    {
        IsFlowing = false;
        isRidden = false;
        oldPosition = transform.position;
    }
    
    void Update()
    {
        //if (flowObj == null) IsFlowing = false;

        //衝突判定はアップデートの前に呼ばれる
        movement = transform.position - oldPosition;
        oldPosition = transform.position;

        //乗っているプレイヤーは一緒に動く
        if(ridingPlayer.Count == 0 || movement == Vector3.zero) return;
        foreach(GameObject obj in ridingPlayer)
        {
            obj.transform.position += movement;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            isRidden = true;
            if (ridingPlayer.Find(n => n.Equals(col.gameObject)) != null) return;
            //リストにいなかったら追加
            ridingPlayer.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            ridingPlayer.Remove(col.gameObject);
            isRidden = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Anchor") return;

        if (flowObj != null && col.transform.position.Equals(flowObj.GetComponent<Flow>().TargetPosition))
        {
            Destroy(flowObj);
            IsFlowing = false;
            flowObj = null;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag != "Anchor") return;

        //既に追加されてたら追加しない
        foreach(GameObject anchor in OnAnchorList)
        {
            if (col.gameObject.Equals(anchor)) return;
        }
        OnAnchorList.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag != "Anchor") return;

        OnAnchorList.Remove(col.gameObject);
    }

}

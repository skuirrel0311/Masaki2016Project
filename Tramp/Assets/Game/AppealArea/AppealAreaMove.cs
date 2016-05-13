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
    /// アンカーに触れているか？
    /// </summary>
    public bool OnAnchor;

    void Start()
    {
        IsFlowing = false;
        isRidden = false;
        OnAnchor = false;
        oldPosition = transform.position;
    }
    
    void Update()
    {
        //衝突判定はアップデートの前に呼ばれる
        movement = transform.position - oldPosition;
        oldPosition = transform.position;

        if(ridingPlayer.Count == 0 || movement == Vector3.zero) return;
        foreach(GameObject obj in ridingPlayer)
        {
            obj.transform.position += movement;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player") PlayerHit(col);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Anchor") AnchorHit(col);
    }

    void AnchorHit(Collision col)
    {
        if (flowObj != null && col.transform.position.Equals(flowObj.GetComponent<Flow>().TargetPosition))
        {
            Destroy(flowObj);
            flowObj = null;
        }
    }

    void PlayerHit(Collider col)
    {
        isRidden = true;
        if (ridingPlayer.Find(n => n.Equals(col.gameObject)) != null) return;
        //リストにいなかったら追加
        ridingPlayer.Add(col.gameObject);
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Anchor") StayAnchor(col);
    }

    void StayAnchor(Collision col)
    {
        //流れの終点と同じアンカーであったら
        if (flowObj != null && col.transform.position.Equals(flowObj.GetComponent<Flow>().TargetPosition))
            OnAnchor = true;
        else
            OnAnchor = false;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            ridingPlayer.Remove(col.gameObject);
            isRidden = false;
        }
    }
}

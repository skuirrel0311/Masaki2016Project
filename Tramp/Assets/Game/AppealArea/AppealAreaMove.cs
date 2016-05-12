using UnityEngine;
using System.Collections;

/*
    ★アピールエリアの動き★
    流れ×：プレイヤー×　＝　中心にもどる
    流れ○：プレイヤー×　＝　流れの終点まで流れる
    流れ×：プレイヤー○　＝　動かない
*/

public class AppealAreaMove : MonoBehaviour
{
    /// <summary>
    /// 流れているか？
    /// </summary>
    bool IsFlowing;
    /// <summary>
    /// プレイヤーに乗られているか？
    /// </summary>
    bool IsRidden;

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
        IsRidden = false;
        OnAnchor = false;
    }
    
    void Update()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Anchor") return;

        if (flowObj != null && col.transform.position.Equals(flowObj.GetComponent<Flow>().TargetPosition))
        {
            Destroy(flowObj);
            flowObj = null;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag != "Anchor") return;

        //流れの終点と同じアンカーであったら
        if (flowObj != null && col.transform.position.Equals(flowObj.GetComponent<Flow>().TargetPosition))
            OnAnchor = true;
        else
            OnAnchor = false;
    }

    void OnCollisionExit(Collision col)
    {
    }
}

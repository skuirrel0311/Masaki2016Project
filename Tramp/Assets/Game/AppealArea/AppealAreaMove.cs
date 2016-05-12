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
    }
    
    void Update()
    {
        if(isRidden)
        {

        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Anchor") AnchorHit(col);

        if (col.gameObject.tag == "Player") PlayerHit(col);
    }

    void AnchorHit(Collision col)
    {
        if (flowObj != null && col.transform.position.Equals(flowObj.GetComponent<Flow>().TargetPosition))
        {
            Destroy(flowObj);
            flowObj = null;
        }
    }

    void PlayerHit(Collision col)
    {
        isRidden = true;
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
        if(col.gameObject.tag == "Player")
        {
            isRidden = false;
        }
    }
}

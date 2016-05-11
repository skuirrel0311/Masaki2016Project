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
        if(col.gameObject.tag == "Anchor")
        {
            Destroy(flowObj);
            flowObj = null;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Anchor")
        {
            OnAnchor = true;
        }
    }

    void OnCollisionExit(Collision col)
    {
    }
}

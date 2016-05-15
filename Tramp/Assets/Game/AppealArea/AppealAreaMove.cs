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
    Vector3 movement;

    AppealAreaState areaState;

    void Start()
    {
        oldPosition = transform.position;
        areaState = GetComponent<AppealAreaState>();
    }
    
    void Update()
    {
        //衝突判定はアップデートの前に呼ばれる
        movement = transform.position - oldPosition;
        oldPosition = transform.position;

        //乗っているプレイヤーは一緒に動く
        if(!areaState.IsRidden || movement == Vector3.zero) return;
        foreach(GameObject obj in areaState.RidingPlayer)
        {
            obj.transform.position += movement;
        }
    }
}

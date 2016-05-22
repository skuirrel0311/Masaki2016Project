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

    [SerializeField]
    float moveSpeed = 3.5f;

    AppealAreaState areaState;

    //初期地に戻ろうとするか？
    bool IsGoHome = false;

    //デバッグ用、trueにすると中心に戻ろうとしなくなります。
    bool IsNoMove = true;

    [SerializeField]
    Vector3 homePosition = Vector3.up;

    [SerializeField]
    float distance = 5;

    float stopDistance = 0.3f;

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
        WithPlayer();

        IsGoHome = (!areaState.IsFlowing && !areaState.IsRidden);

        if (IsGoHome && !IsNoMove)
        {
            areaState.Owner = null;
            GoHome();
        }
    }

    void WithPlayer()
    {
        //乗っているプレイヤーは一緒に動く
        if (!areaState.IsRidden || movement == Vector3.zero) return;
        foreach (GameObject obj in areaState.RidingPlayer)
        {
            obj.transform.position += movement;
        }
    }

    /// <summary>
    /// おうちかえる
    /// </summary>
    void GoHome()
    {
        movement = (homePosition - transform.position);
        float distance = movement.magnitude;
        if (distance < stopDistance) return; //ある程度近づいたらストップ

        movement = movement.normalized * moveSpeed;

        Ray ray = new Ray(transform.position, movement);
        RaycastHit hit;

        //真っ直ぐおうちに帰れなかったら移動量を修正
        if (Physics.SphereCast(ray, 3, out hit,distance))
        {
            //w = movement - (Dot(movement,hit.normal) * hit.normal)
            Vector2 vec1 = new Vector2(movement.x, movement.z);
            Vector2 vec2 = new Vector2(hit.normal.x, hit.normal.z);

            Vector2 vec3 = vec1 + (vec2 * -DotProduct(vec1, vec2));
            movement = new Vector3(vec3.x, movement.y, vec3.y);
            movement = movement.normalized* moveSpeed;
        }
        transform.Translate(movement * Time.deltaTime, Space.World);
    }

    float DotProduct(Vector2 vec1, Vector2 vec2)
    {
        return (vec1.x * vec2.x) + (vec1.y * vec2.y);
    }

    void OnDrawGizmos()
    {
        float radius = 3f;
        RaycastHit hit;
        Vector3 movement = (homePosition - transform.position).normalized * moveSpeed;
        Ray ray = new Ray(transform.position, movement);
        if (Physics.SphereCast(ray, radius, out hit,distance))
        {
            Gizmos.DrawRay(ray.origin, ray.direction * hit.distance);
            Gizmos.DrawWireSphere(ray.origin + ray.direction * (hit.distance), radius);
        }
        else
        {
            Gizmos.DrawRay(ray.origin, ray.direction);
        }
    }
}

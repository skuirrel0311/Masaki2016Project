using UnityEngine;

public class ThroughArea : MonoBehaviour
{
    MeshCollider areaCollider;
    MeshCollider areaTrigger;
    //メッシュがトリガーか？
    bool IsTrigger = false;

    Vector3 oldPlayerPositon;

    //waitのモーションでいた時間
    float waitTime;

    // Use this for initialization
    void Start()
    {
        areaCollider = transform.FindChild("pSphere1").GetComponent<MeshCollider>();
        areaTrigger = transform.FindChild("AppealAreaTrigger").GetComponent<MeshCollider>();
        oldPlayerPositon = Vector3.zero;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag != "Player") return;
        if (IsTrigger) return;
        //床の座標よりもプレイヤーの座標が高ければ戻る
        if ((col.gameObject.transform.position.y + 0.5f > transform.position.y)) return;
        areaCollider.isTrigger = true;
        IsTrigger = true;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag != "Player") return;

        if (areaCollider.isTrigger) return;
        //エリアに乗っているだろう
        Vector3 movement = oldPlayerPositon - col.transform.position;

        if (movement.magnitude != 0) waitTime = 0;
        else waitTime += Time.deltaTime;

        col.GetComponent<PlayerControl>().IsFalling = false;

        PlayerState state = col.GetComponent<PlayerState>();
        if(waitTime > 4) state.IsAppeal = true;
        else state.IsAppeal = false;
        state.animator.SetBool("IsAppeal", state.IsAppeal);


        oldPlayerPositon = col.transform.position;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag != "Player") return;

        if (areaCollider.isTrigger)
        {
            areaCollider.isTrigger = false;
            IsTrigger = false;
        }
        else
        {
            //出て行ってなかったら
            if (!ReallyExit(col.gameObject)) return;

            //出て行った
            col.GetComponent<PlayerControl>().IsOnGround = false;
            col.GetComponent<PlayerControl>().Fall();
            col.GetComponent<PlayerState>().IsAppeal = false;
        }
    }

    //本当にプレイヤーは出て行ったのだろうか？
    bool ReallyExit(GameObject player)
    {
        Ray ray = new Ray(player.transform.position, Vector3.down + player.GetComponent<PlayerControl>().movement);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit,100))
        {
            if (hit.transform.tag == "Area") return false;
            //1より大きかったら出て行った
            return hit.distance > 1;
        }

        //何にも当たらなかったら出て行った
        return true;
    }
}

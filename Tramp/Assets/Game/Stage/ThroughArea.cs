using UnityEngine;

public class ThroughArea : MonoBehaviour
{
    MeshCollider areaCollider;
    MeshCollider areaTrigger;
    //メッシュがトリガーか？
    bool IsTrigger = false;

    // Use this for initialization
    void Start()
    {
        areaCollider = transform.FindChild("pSphere1").GetComponent<MeshCollider>();
        areaTrigger = transform.FindChild("AppealAreaTrigger").GetComponent<MeshCollider>();
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

    void OnTriggerExit(Collider col)
    {
        if (col.transform.tag != "Player") return;
        areaCollider.isTrigger = false;
        IsTrigger = false;
    }
}

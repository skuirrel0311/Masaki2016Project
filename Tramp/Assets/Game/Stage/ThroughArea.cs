using UnityEngine;

public class ThroughArea : MonoBehaviour
{
    MeshCollider[] areaCollider = new MeshCollider[2];
    MeshCollider areaTrigger;
    //メッシュがトリガーか？
    bool IsTrigger = false;

    // Use this for initialization
    void Start()
    {
        areaCollider[0] = transform.FindChild("AppealAreaCollider1").GetComponent<MeshCollider>();
        areaCollider[1] = transform.FindChild("AppealAreaCollider2").GetComponent<MeshCollider>();
        areaTrigger = transform.FindChild("AppealAreaTrigger").GetComponent<MeshCollider>();
    }

    void OnTriggerEnter(Collider col)
    {

        if (col.transform.tag != "Player") return;
        if (IsTrigger) return;
        //床の座標よりもプレイヤーの座標が高ければ戻る
        if ((col.gameObject.transform.position.y + 0.5f > transform.position.y)) return;
        foreach (MeshCollider m in areaCollider) m.isTrigger = true;
        IsTrigger = true;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.transform.tag != "Player") return;
        foreach (MeshCollider m in areaCollider) m.isTrigger = false;
        IsTrigger = false;
    }
}

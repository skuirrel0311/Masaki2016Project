using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

/// <summary>
/// 浮遊する地面にアタッチ
/// これを使用するときはBoxCollisionを大きめかつトリガーに設定し、MeshCollisionをアタッチしておく
/// </summary>
public class throughPlane : MonoBehaviour {

    BoxCollider boxCollider;
    List<MeshCollider> meshColliders = new List<MeshCollider>();

    //メッシュがトリガーか？
    bool IsTrigger = false;
 
	// Use this for initialization
	void Start ()
    {
        boxCollider = GetComponentInParent<BoxCollider>();
        meshColliders.AddRange( GetComponentsInChildren<MeshCollider>());
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag != "Player") return;
        if (!col.GetComponent<PlayerControl>().isLocalPlayer) return;
        if (IsTrigger) return;

        //床の座標よりもプレイヤーの座標が高ければ戻る
        if ((col.gameObject.transform.position.y+0.5f > transform.position.y)) return;
        Debug.Log("Plane Col in");
        meshColliders.ForEach(meth => meth.isTrigger = true);
        IsTrigger = true;
    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log("Plane Tri out");
        if (col.transform.tag != "Player") return;
        meshColliders.ForEach(meth => meth.isTrigger = false);
        IsTrigger = false;
    }
}

using UnityEngine;
using System.Collections;

/// <summary>
/// 浮遊する地面にアタッチ
/// これを使用するときはBoxCollisionを大きめかつトリガーに設定し、MeshCollisionをアタッチしておく
/// </summary>
public class throughPlane : MonoBehaviour {

    BoxCollider boxCollider;
    MeshCollider meshCollider;
 
	// Use this for initialization
	void Start ()
    {
        boxCollider = GetComponent<BoxCollider>();
        meshCollider = GetComponent<MeshCollider>();
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Plane Col in");
        if (col.transform.tag != "Player") return;
        //床の座標よりもプレイヤーの座標が高ければ戻る
        if (col.transform.position.y > transform.position.y) return;

        meshCollider.isTrigger = true;
    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log("Plane Tri out");
        if (col.transform.tag != "Player") return;
        meshCollider.isTrigger = false;
    }
}

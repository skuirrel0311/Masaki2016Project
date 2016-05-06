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

        if (col.transform.tag != "Player") return;
        if (meshCollider.isTrigger) return;
        //床の座標よりもプレイヤーの座標が高ければ戻る
        if ((col.gameObject.transform.position.y > transform.position.y)) return;
        Debug.Log("Plane Col in");
        meshCollider.isTrigger = true;
    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log("Plane Tri out");
        if (col.transform.tag != "Player") return;
        meshCollider.isTrigger = false;
    }
}

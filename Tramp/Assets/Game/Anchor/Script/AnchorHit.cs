using UnityEngine;
using System.Collections;

public class AnchorHit : MonoBehaviour {

    [SerializeField]
    //何回ヒットすれば壊れるか
    int Hp=1;

    [SerializeField]
    GameObject HitEffect;

    /// <summary>
    /// このアンカーから出ている流れ
    /// </summary>
    public GameObject FlowEffect;

    void Start()
    {
        FlowEffect =GameObject.Find("FlowEffect"+CreateFlow.flowEffectCount);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ammo") AmmoHit(col);
    }

    void AmmoHit(Collider col)
    {
        if (col.gameObject.tag == "Ammo")
        {
            Hp--;
            //Hpが0になったらDestroy
            if (Hp <= 0)
            {
                Instantiate(HitEffect,transform.position,transform.rotation);
                Destroy(gameObject);
                Destroy(FlowEffect);
            }
        }
    }

    public  void  Crush()
    {
        Instantiate(HitEffect, transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(FlowEffect);
    }
}

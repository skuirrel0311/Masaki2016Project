using UnityEngine;
using System.Collections;

public class AnchorHit : MonoBehaviour {

    [SerializeField]
    //何回ヒットすれば壊れるか
    int Hp=1;

    [SerializeField]
    GameObject HitEffect;

    GameObject FlowEffect;

    void Start()
    {
        FlowEffect =GameObject.Find("FlowEffect"+CreateFlow.flowEffectCount);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag=="Ammo")
        {
            Hp--;
            //Hpが0になったらDestroy
            if (Hp <= 0)
            {
                Destroy(gameObject);
            }
        }
        else if (col.gameObject.tag=="Player")
        {
            if (FlowEffect!=null)
            {
                FlowEffect.transform.parent = null;
                Destroy(FlowEffect, 2);
            }
            Destroy(gameObject);
            col.gameObject.GetComponent<FeverGauge>().CmdAddPoint(10);
            Instantiate(HitEffect,transform.position,Quaternion.identity);
        }
    }

}

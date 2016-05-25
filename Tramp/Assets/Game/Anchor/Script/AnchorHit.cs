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

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ammo") AmmoHit(col);
        if (col.gameObject.tag == "Player") PlayerHit(col);
        if(col.gameObject.name == "AppealArea") AreaHit(col);
    }
    
    void PlayerHit(Collision col)
    {
        if (FlowEffect != null)
        {
            FlowEffect.transform.parent = null;
            FlowEffect.GetComponent<Flow>().isDestory = true;
        }
        Destroy(gameObject);
        Instantiate(HitEffect, transform.position, Quaternion.identity);
        col.gameObject.GetComponent<PlayerShot>().AnchorHit();
    }

    void AreaHit(Collision col)
    {
        if (FlowEffect != null)
        {
            FlowEffect.transform.parent = null;
            FlowEffect.GetComponent<Flow>().isDestory = true;
            AppealAreaState areaState = col.gameObject.GetComponent<AppealAreaState>();

            Destroy(areaState.flowObj);
            areaState.flowObj = FlowEffect;
            areaState.OnAnchorList.Remove(gameObject);
        }
        Destroy(gameObject);
        Instantiate(HitEffect, transform.position, Quaternion.identity);
    }

    void AmmoHit(Collision col)
    {
        if (col.gameObject.tag == "Ammo")
        {
            Hp--;
            //Hpが0になったらDestroy
            if (Hp <= 0)
            {
                Destroy(gameObject);
                Destroy(FlowEffect);
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class CreateFlow : MonoBehaviour {

    [SerializeField]
    ParticleSystem FlowParticle;

    Vector3 target;
    Vector3 flowVector;
    float collsionRadius=1;

	void Start ()
    {
        GetNearAnchor();

        CreateFlowObject();

	}

    void GetNearAnchor()
    {
        //一番近いアンカーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");
        if (objects.Length <= 0) return;

        float distance = 1000000;

        foreach (GameObject obj in objects)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) < distance)
            {
                target = obj.transform.position;
                flowVector = target - transform.position;
                distance = flowVector.magnitude;
            }
        }

        //アンカーとしてセットする
        gameObject.tag = "Anchor";
    }

    void CreateFlowObject()
    {
        //流れのコリジョン用オブジェクト
        GameObject boxCol = new GameObject("Flow");

        //CapsuleColliderをアタッチする
        boxCol.AddComponent<CapsuleCollider>();
        CapsuleCollider capcol = boxCol.GetComponent<CapsuleCollider>();
        capcol.height = flowVector.magnitude;
        capcol.radius = collsionRadius;
        capcol.isTrigger = true;

        //FlowScriptをアタッチする
        boxCol.AddComponent <Flow>();
        boxCol.GetComponent<Flow>().FlowVector = flowVector;
        //流れのベクトルに合わせて回転させる
        boxCol.transform.position = Vector3.Lerp(target, transform.position, 0.5f);
        boxCol.transform.rotation = Quaternion.FromToRotation(Vector3.up, flowVector.normalized);

        //オブジェクトとの親子関係に加える
        boxCol.transform.parent = transform;

        //流れのパーティクル
        CreateFlowParticle();
    }

    void CreateFlowParticle()
    {
        FlowParticle.startLifetime = 0.2f*flowVector.magnitude;
        //流れのパーティクルをインスタンス、子のオブジェクトとして追加
        ParticleSystem obj= (ParticleSystem)Instantiate(FlowParticle,transform.position, Quaternion.FromToRotation(Vector3.forward, flowVector.normalized));
        obj.transform.parent = transform;
    }

    // Update is called once per frame
    void Update ()
    {
	
	}
}

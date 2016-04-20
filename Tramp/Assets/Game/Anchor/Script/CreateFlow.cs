using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CreateFlow :NetworkBehaviour
{

    [SerializeField]
    ParticleSystem FlowParticle;
    [SerializeField]
    GameObject FlowEffect;

    GameObject targetAnchor;
    Vector3 targetPosition;
    GameObject targetGameObjct;
    [SyncVar]
    Vector3 flowVector;
    float collsionRadius = 1;

    float PlayerIndex=1;

    #region Start

    void Start()
    {
        //targetAnchor = GameObject.Find("Camera").GetComponent<CameraControl>().targetAnchor;

        GetNearAnchor();

        CmdCreateFlowObject();
    }

    public void SetCreatePlayerIndex(int index)
    {
         PlayerIndex = index;
    }

    void GetNearAnchor()
    {
        float distance = 1000000;
        if (targetAnchor != null)
        {
            targetPosition = targetAnchor.transform.position;
            targetGameObjct = targetAnchor;
            flowVector = targetPosition - transform.position;
            distance = flowVector.magnitude;
            return;
        }

        //一番近いアンカーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");
        if (objects.Length <= 0) return;
        foreach (GameObject obj in objects)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) < distance)
            {
                targetPosition = obj.transform.position;
                targetGameObjct = obj;
                flowVector = targetPosition - transform.position;
                distance = flowVector.magnitude;
            }
        }


    }
    [Command]
    void CmdCreateFlowObject()
    {
        //流れのコリジョン用オブジェクト
        GameObject boxCol = Instantiate(FlowEffect);
        boxCol.transform.localScale = new Vector3(2,flowVector.magnitude*0.5f,2);

        //CapsuleColliderをアタッチする
        CapsuleCollider capcol = boxCol.GetComponent<CapsuleCollider>();
        capcol.height = flowVector.magnitude/ (flowVector.magnitude*0.5f);
        capcol.radius = collsionRadius/2;
        capcol.isTrigger = true;

        //FlowScriptをアタッチする
        Flow flow = boxCol.GetComponent<Flow>();
        flow.FlowVector = flowVector;
        flow.TargetPosition = targetPosition;
        //流れのベクトルに合わせて回転させる
        float leapPosition = 0.6f;//このくらいがバグりにくいと思われる
        boxCol.transform.position = Vector3.Lerp(targetPosition, transform.position, leapPosition);
        boxCol.transform.rotation = Quaternion.FromToRotation(Vector3.up, flowVector.normalized);

        //オブジェクトとの親子関係に加える
        boxCol.transform.parent = transform;
        NetworkServer.Spawn(boxCol);
        //流れのパーティクル
        CreateFlowParticle();

        //アンカーとしてセットする
        gameObject.tag = "Anchor";
    }

    void CreateFlowParticle()
    {
        FlowParticle.startLifetime = 0.2f * flowVector.magnitude;
        //流れのパーティクルをインスタンス、子のオブジェクトとして追加
        ParticleSystem ps = (ParticleSystem)Instantiate(FlowParticle, transform.position, Quaternion.FromToRotation(Vector3.forward, flowVector.normalized));
        ps.transform.parent = transform;
        //NetworkServer.Spawn(ps);
    }
    #endregion

    #region Update
    void Update()
    {
        DestroyChiled();

    }

    //流れの参照先のオブジェクトが存在していなければ流れを消す
    void DestroyChiled()
    {
        if (targetGameObjct != null) return;

        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
    #endregion
}

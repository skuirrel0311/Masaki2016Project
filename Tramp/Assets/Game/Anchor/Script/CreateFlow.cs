using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CreateFlow : NetworkBehaviour
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

    float PlayerIndex = 1;

    public static int flowEffectCount = 0;

    #region Start

    void Start()
    {
        targetAnchor = GameObject.Find("Camera1").GetComponent<CameraControl>().targetAnchor;

        GetNearAnchor();

        //アンカーとしてセットする
        gameObject.tag = "Anchor";
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

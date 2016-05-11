using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Flow : NetworkBehaviour{

    [SerializeField]
    private float speed=10;
    [SerializeField]
    bool nonDestroy;

    public Vector3 FlowVector
    {
        get { return flowVector; }
        set { flowVector = value; isCalc = true; }
    }
    [SyncVar]
    private Vector3 flowVector;

    private Vector3 PlayerVector;

    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; isCalc = true; }
    }
    [SyncVar]
    private Vector3 targetPosition;

    private bool isCalc = true;

    public bool isDestory;

    void Awake()
    {
        CreateFlow.flowEffectCount++;
        gameObject.name = "FlowEffect"+CreateFlow.flowEffectCount;
    }

    void Start()
    {
        isCalc = false;
        isDestory = false;
    }
    void Update()
    {
        // if (!isCalc) return;
        transform.localScale = new Vector3(2, flowVector.magnitude * 0.5f, 2);
        //CapsuleColliderをアタッチする
        CapsuleCollider capcol = GetComponent<CapsuleCollider>();
        capcol.height = flowVector.magnitude / (flowVector.magnitude * 0.5f);
        capcol.radius = 0.5f;
        capcol.isTrigger = true;
    }

    void OnTriggerStay(Collider col)
    {

        if (col.tag == "Player")
        {
            PlayerVector = targetPosition - (col.transform.position + Vector3.up);
            PlayerVector.Normalize();
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            body.isKinematic = true;
            col.gameObject.transform.Translate(PlayerVector*Time.deltaTime*speed,Space.World);

            if (nonDestroy) return;
            //ターゲットから一定の距離
            if (Vector3.Distance(targetPosition, col.gameObject.transform.position) < 2)
            {
                Destroy(gameObject); return;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (nonDestroy) return;
        if (col.tag == "Player")
        {
            if (isDestory) { Destroy(gameObject); return; }


        }
    }
}

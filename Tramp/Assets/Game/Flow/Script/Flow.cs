using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

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

    public GameObject targetAnchor;

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

        if (targetAnchor == null) GetTargetAnchor();
        transform.localScale = new Vector3(2, flowVector.magnitude * 0.5f, 2);

        CapsuleCollider capcol = GetComponent<CapsuleCollider>();
        capcol.height = flowVector.magnitude / (flowVector.magnitude * 0.5f);
        capcol.radius = 0.5f;
        capcol.isTrigger = true;
    }

    void GetTargetAnchor()
    {
        List<GameObject> anchor = new List<GameObject>();
        anchor.AddRange(GameObject.FindGameObjectsWithTag("Anchor"));

        targetAnchor = anchor.Find(n => n.transform.position.Equals(targetPosition));
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "AppealArea")
        {
            AppealAreaState appealArea = col.gameObject.GetComponent<AppealAreaState>();

            if (!appealArea.IsFlowing)
            {
                //アピールエリアのflowObjをこの流れに設定して終了
                appealArea.flowObj = gameObject;
                return;
            }

            ////Enterなので同じ流れになることは無いが一応
            //if (appealArea.flowObj.Equals(gameObject)) return;

            ////違う流れに入った
            ////違う流れのほうに流れるので今流れているほうは除外する
            //Destroy(appealArea.flowObj);
            //appealArea.flowObj = gameObject;
        }
    }

    void OnTriggerStay(Collider col)
    {

        if (col.tag == "Player")
        {
            PlayerState state = col.gameObject.GetComponent<PlayerState>();
            //アピールエリアのFlowObjectと同じだったら流れない
            GameObject flowObj = state.AppealArea.flowObj;
            if (flowObj != null && flowObj.Equals(gameObject)) return;
            //アピールエリアにいたら流れない
            if (state.IsOnAppealArea) return;

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

        if (col.gameObject.name == "AppealArea")
        {
            AppealAreaState appealArea = col.gameObject.GetComponent<AppealAreaState>();

            //まだ流れていなかったら
            if (!appealArea.IsFlowing) appealArea.flowObj = gameObject;

            //流れの終点アンカーとアピールエリアが触れているアンカーが同じだったら流れない
            foreach(GameObject anchor in appealArea.OnAnchorList)
            {
                if (anchor.Equals(targetAnchor)) return;
            }

            //違う流れには乗らない
            if (!appealArea.flowObj.Equals(gameObject)) return;

            //流れに乗る
            appealArea.IsFlowing = true;
            Vector3 toAnchorVector = (targetPosition - col.transform.position).normalized;
            col.gameObject.transform.Translate(toAnchorVector * Time.deltaTime * (speed * 0.1f), Space.World);       
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (nonDestroy) return;
        if (col.tag == "Player")
        {
            if (isDestory) { Destroy(gameObject); return; }
        }

        if(col.name == "AppealArea")
        {
            col.gameObject.GetComponent<AppealAreaState>().IsFlowing = false;

            if(isDestory) Destroy(gameObject);
        }
    }
}

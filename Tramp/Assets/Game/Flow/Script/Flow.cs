using UnityEngine;
using UnityEngine.Networking;

public class Flow : NetworkBehaviour{

    private float speed=10;

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

        if (targetAnchor == null) Destroy(gameObject);
        transform.localScale = new Vector3(2, flowVector.magnitude * 0.5f, 2);

        CapsuleCollider capcol = GetComponent<CapsuleCollider>();
        capcol.height = flowVector.magnitude / (flowVector.magnitude * 0.5f);
        capcol.radius = 0.5f;
        capcol.isTrigger = true;
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
            PlayerVector = targetPosition - (col.transform.position + Vector3.up);
            PlayerVector.Normalize();
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            body.isKinematic = true;
            col.gameObject.transform.Translate(PlayerVector*Time.deltaTime*speed,Space.World);
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
            col.gameObject.transform.Translate(toAnchorVector * Time.deltaTime * (speed * 0.5f), Space.World);       
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            if (isDestory) { Destroy(gameObject); }
        }

        if(col.name == "AppealArea")
        {
            col.gameObject.GetComponent<AppealAreaState>().IsFlowing = false;

            if(isDestory) Destroy(gameObject);
        }
    }
}

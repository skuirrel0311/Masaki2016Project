using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Flow : NetworkBehaviour{

    [SerializeField]
    private float speed=10;
    [SerializeField]
    bool nonDestroy = false;

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
    public GameObject startAnchor;

    private bool isCalc = true;

    public bool isDestory;

    /// <summary>
    /// アピールエリアから繋がっている流れか？
    /// </summary>
    [SyncVar]
    public bool IsFromArea = false;

    /// <summary>
    /// アピールエリアへ繋ぐ流れか？
    /// </summary>
    public bool IsToArea = false;

    private GameObject appealArea;

    void Awake()
    {
        CreateFlow.flowEffectCount++;
        gameObject.name = "FlowEffect"+CreateFlow.flowEffectCount;
    }

    void Start()
    {
        isCalc = false;
        isDestory = false;

        if (IsFromArea)
        {
            appealArea = GameObject.Find("AppealArea");
            AppealAreaState areaState = appealArea.GetComponent<AppealAreaState>();
            if (areaState.flowObj != null)
                Destroy(areaState.flowObj);
            areaState.flowObj = gameObject;
        }

        if(IsToArea)
        {
            targetAnchor = GameObject.Find("AreaAnchor");
        }
    }
    void Update()
    {
        // if (!isCalc) return;

        if (targetAnchor == null) GetTargetAnchor();
        if (startAnchor == null)
            GetStartAnchor();
        
        if(IsFromArea)
        {
            flowVector = targetPosition - appealArea.transform.position;
            
            transform.position = Vector3.Lerp(targetPosition,appealArea.transform.position,0.5f);
        }
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

        if (targetAnchor == null)
            Destroy(gameObject);
    }

    //始点を探す
    void GetStartAnchor()
    {
        List<GameObject> anchor = new List<GameObject>();
        anchor.AddRange(GameObject.FindGameObjectsWithTag("Anchor"));

        if (anchor.Count == 0) return;
        else Debug.Log("showAnchor");

        anchor = anchor.FindAll(n => n.name != "FixAnchor" && n.name != "AreaAnchor");

        if (anchor.Count == 0) return;

        startAnchor = anchor.Find(n => n.GetComponent<AnchorHit>().FlowEffect.Equals(gameObject));

        if (startAnchor == null) return;
        else Debug.Log("getAnchor");
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player") PlayerStay(col);
        if (col.gameObject.name == "AppealArea") AreaStay(col);
    }

    void PlayerStay(Collider col)
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
        col.gameObject.transform.Translate(PlayerVector * Time.deltaTime * speed, Space.World);

        if (nonDestroy) return;
        //ターゲットから一定の距離
        if (Vector3.Distance(targetPosition, col.gameObject.transform.position) < 2)
        {
            Destroy(gameObject);
            return;
        }
    }

    void AreaStay(Collider col)
    {
        AppealAreaState appealArea = col.gameObject.GetComponent<AppealAreaState>();

        //flowObjは流れを繋いだときに代入
        //まだ流れていなかったら
        if (appealArea.flowObj == null) return;

        //流れの終点アンカーとアピールエリアが触れているアンカーが同じだったら流れない
        foreach (GameObject anchor in appealArea.OnAnchorList)
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
    
    void OnTriggerExit(Collider col)
    {
        if (nonDestroy) return;
        if (col.tag == "Player")
        {
            if (isDestory) {
                Destroy(gameObject); return; }
        }

        if(col.name == "AppealArea")
        {
            col.gameObject.GetComponent<AppealAreaState>().IsFlowing = false;

            if(isDestory)
                Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (IsFromArea) return;

        Destroy(startAnchor);
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Flow : NetworkBehaviour
{

    [SerializeField]
    private float speed = 10;
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

    List<Rigidbody> bodys = new List<Rigidbody>();

    //この流れがどちらのプレイヤーによって作られているか
    public bool WhichCreatePlayer
    {
        get { return whichCreatePlayer; }
        set { whichCreatePlayer = value; }
    }
    [SyncVar]
    private bool whichCreatePlayer;

    //作ったプレイヤーか？
    public bool isCreatePlayer
    {
        get { return isServer == WhichCreatePlayer; }
    }


    private GameObject appealArea;
    bool isrenderd;

    void Awake()
    {
        CreateFlow.flowEffectCount++;
        gameObject.name = "FlowEffect" + CreateFlow.flowEffectCount;
    }

    void Start()
    {
        isCalc = true;
        isDestory = false;
        isrenderd = true;

        if (!isCreatePlayer)
        {
            StopFlowRender();
        }

        //作ったプレイヤーに合わせて色を替える
        if (whichCreatePlayer)
        {
            GetComponent<Renderer>().materials[0].SetColor("_Color", Color.blue);
        }
        else
        {
            GetComponent<Renderer>().materials[0].SetColor("_Color", new Color(0.5f, 0, 0));
        }

        //接続先が固定のアンカーだったら自分の情報を固定のアンカーに送るふぃｘ
        List<GameObject> gos = new List<GameObject>();
        gos.AddRange(GameObject.FindGameObjectsWithTag("Anchor"));
        GameObject go = gos.Find(anchor => anchor.transform.position.Equals(targetPosition));
        if (go == null) return;
        if (go.name == "AreaAnchor")
        {
            go.GetComponent<FixAnchorHit>().ConnectionFlow(gameObject);
        }
    }

    public void FlowRender()
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<LineRenderer>().enabled = true;
    }
    public void StopFlowRender()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<LineRenderer>().enabled = false;
        isrenderd = false;
    }

    void Update()
    {

        if (targetAnchor == null) GetTargetAnchor();
        //if (startAnchor == null)
        //    GetStartAnchor();

        if (!isCreatePlayer && isrenderd == true)
        {
            GetComponent<Renderer>().enabled = false;
            GetComponent<LineRenderer>().enabled = false;
        }

        if (!isCalc) return;
        transform.localScale = new Vector3(2, flowVector.magnitude * 0.5f + 1.0f, 2);
        GetComponent<MeshRenderer>().materials[0].SetFloat("_LineNum", flowVector.magnitude);
        CapsuleCollider capcol = GetComponent<CapsuleCollider>();
        capcol.height = flowVector.magnitude / (flowVector.magnitude * 0.5f);
        capcol.radius = 0.5f;
        GetComponent<LineRenderer>().SetPosition(0, transform.position + (transform.up * (transform.localScale.y-2)));
        GetComponent<LineRenderer>().SetPosition(1, transform.position - (transform.up * (transform.localScale.y-2)));
        capcol.isTrigger = true;
        isCalc = false;
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

        anchor = anchor.FindAll(n => n.name == "InstanceAnchor(Clone)");

        if (anchor.Count == 0) return;

        //startAnchor = anchor.Find(n => n.GetComponent<AnchorHit>().FlowEffect.name == gameObject.name);

        //if (startAnchor == null) return;
        //else Debug.Log("getAnchor");
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player") return;
        if (!col.gameObject.GetComponent<NetworkBehaviour>().isLocalPlayer) return;
        if (col.gameObject.GetComponent<PlayerControl>().hitFix) return;
        FlowRender();
        Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
        bodys.Add(body);
        body.useGravity = false;
        body.velocity = transform.up * body.velocity.magnitude;
        col.gameObject.GetComponent<Animator>().CrossFadeInFixedTime("ride", 0.1f);
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player") PlayerStay(col);
    }

    void PlayerStay(Collider col)
    {
        if (col.gameObject.GetComponent<PlayerControl>().hitFix) return;
        PlayerState state = col.gameObject.GetComponent<PlayerState>();
        col.gameObject.GetComponent<PlayerControl>().IsFlowing = true;
        if (isCreatePlayer)

        {
            col.gameObject.GetComponent<Penetrate>().Energy++;
        }
        else
        {
            col.gameObject.GetComponent<Penetrate>().Energy--;
        }

        PlayerVector = targetPosition - (col.transform.position + Vector3.up);
        PlayerVector.Normalize();
        Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
        body.useGravity = false;
        body.AddForce(PlayerVector * Time.deltaTime * speed * 50, ForceMode.Acceleration);


    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag != "Player") return;
        Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
        body.useGravity = true;
        bodys.Remove(body);

        PlayerControl control = col.GetComponent<PlayerControl>();
        control.IsFlowing = false;
        control.IsFalling = true;
        CameraControl cam = GameObject.Find("Camera1").GetComponent<CameraControl>();
        cam.SetNowLatitude();
        cam.IsEndFallingCamera = false;

        if (!isLocalPlayer) return;
        if (!isCreatePlayer) isrenderd = false;
    }

    void OnDestroy()
    {
        foreach (Rigidbody body in bodys)
        {
            body.useGravity = true;
        }
    }
}

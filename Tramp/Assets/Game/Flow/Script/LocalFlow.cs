using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LocalFlow : MonoBehaviour{

    [SerializeField]
    private float speed = 10;
    [SerializeField]
    bool nonDestroy = false;

    private bool isCalc = true;

    public bool isDestory;

    List<Rigidbody> bodys = new List<Rigidbody>();

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
    }

    void FixedUpdate()
    {
        if (bodys.Count <= 0) return;
        foreach (Rigidbody go in bodys)
        {
            if (go.gameObject.GetComponent<PlayerControl>().hitFix) continue;
            Rigidbody body = go.gameObject.GetComponent<Rigidbody>();
            body.useGravity = false;
            body.AddForce(transform.up* Time.deltaTime * speed * 50, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        if (!isCalc) return;
        GetComponent<MeshRenderer>().materials[0].SetFloat("_LineNum", transform.localScale.y*2);
        CapsuleCollider capcol = GetComponent<CapsuleCollider>();
        capcol.height = transform.localScale.y * 2 / (transform.localScale.y * 2 * 0.5f);
        capcol.radius = 0.5f;
        GetComponent<LineRenderer>().SetPosition(0, transform.position + (transform.up * (transform.localScale.y*0.6f)));
        GetComponent<LineRenderer>().SetPosition(1, transform.position - (transform.up * (transform.localScale.y*0.6f)));
        capcol.isTrigger = true;
        isCalc = false;
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player") return;

        Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
        bodys.Add(body);
        if (col.gameObject.GetComponent<PlayerControl>().hitFix) return;
        body.useGravity = false;
        body.velocity = transform.up * body.velocity.magnitude;
        if (col.gameObject.GetComponent<PlayerState>().ISDead) return;
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
        col.gameObject.GetComponent<Penetrate>().Energy++;
        col.GetComponent<PlayerControl>().IsJumping = false;
    }


    void OnTriggerExit(Collider col)
    {
        if (col.tag != "Player") return;
        Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
        body.useGravity = true;
        bodys.Remove(body);

        PlayerControl control = col.GetComponent<PlayerControl>();
        control.IsFlowing = false;

        if (control.IsOnGround)
        {
            control.Landed();
            return;
        }

        control.IsFalling = true;
        control.IsJumping = true;
        CameraControl cam = GameObject.Find("Camera1").GetComponent<CameraControl>();
        col.gameObject.GetComponent<Animator>().CrossFadeInFixedTime("jump", 0.5f);
        cam.SetNowLatitude();
        cam.IsEndFallingCamera = false;
    }
}

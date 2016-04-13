using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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

    private bool isCalc = false;
    void Start()
    {
        isCalc = false;
    }
    void Update()
    {
        if (!isCalc) return;
        transform.localScale = new Vector3(2, flowVector.magnitude * 0.5f, 2);
        //CapsuleColliderをアタッチする
        CapsuleCollider capcol = GetComponent<CapsuleCollider>();
        capcol.height = flowVector.magnitude / (flowVector.magnitude * 0.5f);
        capcol.radius = 0.5f;
        capcol.isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            PlayerVector = targetPosition - (col.transform.position+Vector3.up);
            PlayerVector.Normalize();
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            body.useGravity = false;
            col.gameObject.transform.Translate(PlayerVector*Time.deltaTime*speed,Space.World);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            body.useGravity = true;
        }
    }
}

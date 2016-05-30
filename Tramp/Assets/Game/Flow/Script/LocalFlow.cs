using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LocalFlow : MonoBehaviour{

    [SerializeField]
    private float speed=10;
    [SerializeField]
    bool nonDestroy;

    public Vector3 FlowVector
    {
        get { return flowVector; }
        set { flowVector = value; }
    }
    
    private Vector3 flowVector;

    private Vector3 PlayerVector;



    [SerializeField]
    Transform target;


    void Awake()
    {
        CreateFlow.flowEffectCount++;
        gameObject.name = "FlowEffect"+CreateFlow.flowEffectCount;
    }

    void Start()
    {
        GetComponent<MeshRenderer>().materials[0].SetFloat("_LineNum", transform.localScale.y*2);
        GetComponent<LineRenderer>().SetPosition(0, transform.position + (transform.up * transform.localScale.y));
        GetComponent<LineRenderer>().SetPosition(1, transform.position - (transform.up * transform.localScale.y));

    }


    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            col.gameObject.GetComponent<Animator>().CrossFadeInFixedTime("ride",0.1f);
            //body.isKinematic = true;
            //col.gameObject.transform.Translate(transform.up*Time.deltaTime*speed,Space.World);
            body.AddForce(transform.up * Time.deltaTime * speed*100, ForceMode.Impulse);
        }
    }

    void OnTriggerStay(Collider col)
    {

        if (col.tag == "Player")
        {
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            body.AddForce(transform.up * Time.deltaTime * speed,ForceMode.Acceleration);
            
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
           // body.velocity = Vector3.zero;
        }
    }
}

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
        GetComponent<LineRenderer>().SetPosition(0, transform.position + (transform.up * transform.localScale.y*0.5f));
        GetComponent<LineRenderer>().SetPosition(1, transform.position - (transform.up * transform.localScale.y*0.5f));

    }


    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            col.gameObject.GetComponent<Animator>().CrossFadeInFixedTime("ride",0.1f);
            body.AddForce(transform.up * Time.deltaTime * speed*100, ForceMode.Impulse);
        }
    }

    void OnTriggerStay(Collider col)
    {

        if (col.tag == "Player")
        {
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            body.AddForce(transform.up * Time.deltaTime * speed,ForceMode.Acceleration);
            col.gameObject.GetComponent<PlayerControl>().IsFlowing = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            PlayerControl control = col.GetComponent<PlayerControl>();
            control.IsFlowing = false;
            control.IsFalling = true;
            CameraControl cam = GameObject.Find("Camera1").GetComponent<CameraControl>();
            cam.SetNowLatitude();
            cam.IsEndFallingCamera = false;
        }
    }
}

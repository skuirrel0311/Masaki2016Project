using UnityEngine;
using System.Collections;

public class Flow : MonoBehaviour {

    private float speed=10;

    public Vector3 FlowVector
    {
        get { return flowVector; }
        set { flowVector = value.normalized; }
    }
    private Vector3 flowVector;


	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Anchor")
        {
            
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag != "Anchor")
        {
            col.gameObject.transform.Translate(flowVector*Time.deltaTime*speed);
        }
    }
}

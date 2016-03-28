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

    private Vector3 PlayerVector;

    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; }
    }
    private Vector3 targetPosition;

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
        if (col.tag == "Ammo")
        {
            col.gameObject.transform.Translate(FlowVector * Time.deltaTime * speed, Space.World);
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

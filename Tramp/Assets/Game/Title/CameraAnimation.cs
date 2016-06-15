using UnityEngine;
using System.Collections;

public class CameraAnimation : MonoBehaviour
{

    [SerializeField]
    float radius=30;

    [SerializeField]
    float Speed = 0.5f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float spe = Time.time * Speed;

        transform.position  = new Vector3(Mathf.Sin(spe)*radius,transform.position.y,Mathf.Cos(spe)*radius);


        transform.LookAt(Vector3.zero);


    }
}

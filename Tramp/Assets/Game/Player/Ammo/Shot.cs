using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour
{

    [SerializeField]
    float speed = 10;

    [SerializeField]
    GameObject HitEffect;

    Rigidbody body;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        body.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision col)
    {
        //if (col.gameObject.tag == "Player") return;
        Debug.Log("Ammohit");
        Instantiate(HitEffect,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }

    void OnCollisionStay(Collision col)
    {
        //if (col.gameObject.tag == "Player") return;
        Debug.Log("Ammohit");
        Instantiate(HitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

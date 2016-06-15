using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Shot : MonoBehaviour
{

    [SerializeField]
    float speed = 0.1f;

    [SerializeField]
    GameObject HitEffect;

    public bool isLocal=false;

    Rigidbody body;
    Vector3 movement;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, 5);
        movement = transform.forward * speed;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = movement;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player") return;
        if (col.gameObject.tag == "Ammo") return;
        Debug.Log("Ammohit");
        Instantiate(HitEffect,transform.position,transform.rotation);
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Shot : MonoBehaviour
{

    [SerializeField]
    float speed = 0.1f;

    [SerializeField]
    GameObject HitEffect;

    [SerializeField]
    GameObject barrierEffect;
    
    [SerializeField]
    private float EndArea = 47.2f;

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

        Vector3 temp = new Vector3(transform.position.x + (movement.x * 0.1f), 0, transform.position.z + (movement.z * 0.1f));

        if (temp.magnitude > EndArea)
        {
            OutStage();
        }
    }

    public void OutStage()
    {
        Vector3 mov = transform.position - Vector3.zero;
        mov = (mov * 1.01f);
        float effectRotationY = Mathf.Atan2(mov.x, mov.z) * Mathf.Rad2Deg;
        for (int i = 0; i < 5; i++){
            Destroy(Instantiate(barrierEffect, mov, Quaternion.Euler(0, effectRotationY, 0)), 0.3f);
        }
        Destroy(gameObject);
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

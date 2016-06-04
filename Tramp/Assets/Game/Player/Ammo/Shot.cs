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

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, 10);
        GetComponent<Rigidbody>().AddForce(transform.forward * speed,ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
       // transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
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

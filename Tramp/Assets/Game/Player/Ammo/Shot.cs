using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour
{

    [SerializeField]
    float speed = 10;

    [SerializeField]
    GameObject HitEffect;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision col)
    {
        //if (col.gameObject.tag == "Player") return;
        Debug.Log("Ammohit");
        Instantiate(HitEffect,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}

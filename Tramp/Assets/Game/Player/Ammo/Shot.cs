using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {

    [SerializeField]
    float speed=10;

	// Use this for initialization
	void Start () {
        Destroy(gameObject,10);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(transform.forward*speed*Time.deltaTime,Space.World);
	}
}

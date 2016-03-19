using UnityEngine;
using System.Collections;

public class PlayerCreateAnchor : MonoBehaviour {

    [SerializeField]
    GameObject InstanceAnchor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(InstanceAnchor,transform.position+transform.forward,transform.rotation);
        }
	}
}

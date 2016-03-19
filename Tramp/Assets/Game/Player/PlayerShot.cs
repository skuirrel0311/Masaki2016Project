using UnityEngine;
using System.Collections;

public class PlayerShot : MonoBehaviour {

    [SerializeField]
    GameObject Ammo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire2"))
        {
            Instantiate(Ammo,transform.position+Vector3.up,transform.rotation);
        }
	}
}

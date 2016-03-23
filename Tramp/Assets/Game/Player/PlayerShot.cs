using UnityEngine;
using System.Collections;

public class PlayerShot : MonoBehaviour {

    [SerializeField]
    GameObject Ammo;

    [SerializeField]
    GameObject Nozzle;

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        float vertical=0;
        //vertical = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.R))
        {
            vertical = 1.0f;
        }
        else  if(Input.GetKey(KeyCode.F))
        {
            vertical = -1.0f;
        }

        Nozzle.transform.Rotate(vertical,0,0);

        if (Input.GetButtonDown("Fire2"))
        {
            Instantiate(Ammo,transform.position+Vector3.up,Nozzle.transform.rotation);
        }
	}
}

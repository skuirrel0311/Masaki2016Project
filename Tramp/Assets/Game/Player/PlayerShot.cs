using UnityEngine;
using System.Collections;

public class PlayerShot : MonoBehaviour {

    [SerializeField]
    GameObject Ammo;

    [SerializeField]
    GameObject Nozzle;

    [SerializeField]
    float MaxAngle = 30;

    private Vector3 NozzleAngle;

    // Use this for initialization
    void Start ()
    {
        NozzleAngle = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float vertical=0;
        float horizontal=0;

        if (Input.GetKey(KeyCode.R))
        {
            vertical = 1.0f;
        }
        else if (Input.GetKey(KeyCode.F))
        {
            vertical = -1.0f;
        }

        vertical = Input.GetAxis("Vertical2");
        horizontal = Input.GetAxis("Horizontal2");

        NozzleAngle += new Vector3(vertical,horizontal,0);
        //制限内であれば照準を自由に移動
        if (NozzleAngle.magnitude < MaxAngle)
        {
            Nozzle.transform.Rotate(vertical, horizontal, 0);
        }
        //制限に引っかかれば制限する
        else
        {
            NozzleAngle = NozzleAngle.normalized * MaxAngle;
            Nozzle.transform.localEulerAngles = NozzleAngle;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Instantiate(Ammo,Nozzle.transform.position,Nozzle.transform.rotation);
        }
	}


}

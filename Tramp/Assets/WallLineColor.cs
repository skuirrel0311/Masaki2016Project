using UnityEngine;
using System.Collections;

public class WallLineColor : MonoBehaviour {

    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Renderer>().materials[0].EnableKeyword("_EMISSION");
        GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", new Color(0,Mathf.Sin((Time.time%Mathf.PI)) * 0.8f, Mathf.Sin( Time.time%Mathf.PI)));
	}
}

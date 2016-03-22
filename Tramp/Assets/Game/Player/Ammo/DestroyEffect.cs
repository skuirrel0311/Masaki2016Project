using UnityEngine;
using System.Collections;

public class DestroyEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //エフェクトが出てから0.5秒で破棄
        Destroy(this.gameObject,0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

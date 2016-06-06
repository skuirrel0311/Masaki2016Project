using UnityEngine;
using System.Collections;

public class DestroyEffect : MonoBehaviour {

    [SerializeField]
    float DestroyTime = 0.5f;

	// Use this for initialization
	void Start () {
        //エフェクトが出てから0.5秒で破棄
        Destroy(this.gameObject,DestroyTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

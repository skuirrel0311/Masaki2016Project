using UnityEngine;
using System.Collections;

public class SpriteRotate : MonoBehaviour {

    RectTransform rectTransform;

	// Use this for initialization
	void Start () {
        rectTransform = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        rectTransform.Rotate(1,1,1);
	}
}

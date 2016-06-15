using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    [SerializeField]
    float FadeTime = 1000;

    Image image;

	void Start ()
    {
        image = GetComponent<Image>();
        image.CrossFadeColor(Color.clear,FadeTime,false,true);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}

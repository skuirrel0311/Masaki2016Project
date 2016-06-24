using UnityEngine;
using System.Collections;

public class PopUp : MonoBehaviour {

    RectTransform r_Transform;
    float timer;

    public static bool isPopUp=false; 
	// Use this for initialization
	void Start ()
    {
        r_Transform = GetComponent<RectTransform>();
        r_Transform.localScale = new Vector3(0,0,1);
        timer = 0;
        isPopUp = true;
	}

    void OnDisable()
    {
        isPopUp = false;
    }

    void OnDestroy()
    {
        isPopUp = false;
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        timer += Time.deltaTime * 5;
        r_Transform.localScale = Vector3.Lerp(Vector3.forward,Vector3.one,timer);

        if(GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.A,GamepadInput.GamePadInput.Index.One))
        {
            gameObject.SetActive(false);
            isPopUp = false;
        }
	}
}

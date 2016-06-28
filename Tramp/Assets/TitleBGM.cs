using UnityEngine;
using System.Collections;

public class TitleBGM : MonoBehaviour {
    public static bool dontDestroy= true;
	void Start ()
    {
        if (dontDestroy) DontDestroyOnLoad(this);
        dontDestroy = false;
    }
}

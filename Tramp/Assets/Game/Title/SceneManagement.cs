using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {

    AsyncOperation mainScene;

	// Use this for initialization
	void Start () {
        //シーンを予め読み込んでおく
        mainScene = SceneManager.LoadSceneAsync ("GPGPUtest");
        mainScene.allowSceneActivation = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            //シーン遷移を開始する
            mainScene.allowSceneActivation = true;
        }
	}
}

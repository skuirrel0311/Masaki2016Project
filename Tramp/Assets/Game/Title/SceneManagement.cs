using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour {

    AsyncOperation mainScene;

    [SerializeField]
    string MoveToSceneName="GPGPUtest";

	// Use this for initialization
	void Start () {
        //シーンを予め読み込んでおく
        mainScene = SceneManager.LoadSceneAsync(MoveToSceneName);
        mainScene.allowSceneActivation = false;
    }

    // Update is called once per frame
    void Update () {
        if (GamepadInput.GamePadInput.GetButton(GamepadInput.GamePadInput.Button.A,0))
        {
            //シーン遷移を開始する
            mainScene.allowSceneActivation = true;
        }
	}
}

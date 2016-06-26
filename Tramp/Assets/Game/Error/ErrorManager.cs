using UnityEngine;
using GamepadInput;
using System.Collections;

public class ErrorManager : MonoBehaviour {

    MyNetworkManager networkManager;

	// Use this for initialization
	void Start ()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetworkManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool inputA = GamePadInput.GetButtonDown(GamePadInput.Button.A,GamePadInput.Index.One);

        if(inputA)
        {
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            networkManager.offlineScene = "Menu";
            networkManager.ServerChangeScene("Menu");
        }
	}
}

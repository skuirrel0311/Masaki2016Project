using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkRotation : NetworkBehaviour {

    [SyncVar]
    private Quaternion syncPlayerRotation;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private float lerpRate = 15;

    void Start()
    {
        playerTransform=transform;
    }

	// Update is called once per frame
	void FixedUpdate() {
        TransmitRotation();
        LerpRotation();
	}

    [Client]
    void TransmitRotation()
    {
        if (isLocalPlayer)
        {
            CmdProvideRotation(playerTransform.rotation);
        }
    }

    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation,syncPlayerRotation,Time.deltaTime*lerpRate);
        }
    }

    [Command]
    void CmdProvideRotation(Quaternion rotation)
    {
        syncPlayerRotation = rotation;
    }


}

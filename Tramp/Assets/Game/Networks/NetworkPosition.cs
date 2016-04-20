using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkPosition : NetworkBehaviour
{

    [SyncVar]
    Vector3 syncPosition;

    [SerializeField]
    private Transform myTransform;

    [SerializeField]
    private float lerpRate = 15;

    void Start()
    {
        myTransform = transform;
    }

	void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
	}

    /// <summary>
    /// 二点間を補間する
    /// </summary>
    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(myTransform.position,syncPosition,Time.deltaTime*lerpRate);
        }
    }

    /// <summary>
    ///positionの情報をホストに送る
    /// </summary>
    /// <param name="pos"></param>
    [Command]
    void CmdProvidePosition(Vector3 pos)
    {
        syncPosition = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            CmdProvidePosition(myTransform.position);
        }
    }
}

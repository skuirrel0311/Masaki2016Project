using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AreaNetworkPosition : NetworkBehaviour
{

    [SyncVar]
    Vector3 syncPosition;

    [SerializeField]
    private Transform myTransform;

    [SerializeField]
    private float lerpRate = 15;

    [SerializeField]
    private float threshold = 0.5f;

    private Vector3 lastPosition;

    void Start()
    {
        myTransform = transform;
        lastPosition = myTransform.position;
        syncPosition = myTransform.position;
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
        if (isServer) return;
        myTransform.position = Vector3.Lerp(myTransform.position, syncPosition, Time.deltaTime * lerpRate);
    }

    /// <summary>
    ///positionの情報をホストに送る
    /// </summary>
    /// <param name="pos"></param>
    [Command]
    void CmdProvidePosition(Vector3 pos)
    {
        syncPosition = pos;
        lastPosition = pos;
    }

    [Client]
    void TransmitPosition()
    {
        if (!isServer) return;
        if (Vector3.Distance(myTransform.position, lastPosition) > threshold)
            CmdProvidePosition(myTransform.position);

        lastPosition = myTransform.position;
    }

    public void ClientMove(Vector3 pos)
    {
        CmdProvidePosition(pos);
    }

}

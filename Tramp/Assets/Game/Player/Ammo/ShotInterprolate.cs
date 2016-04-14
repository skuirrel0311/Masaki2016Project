using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShotInterprolate : NetworkBehaviour
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
        PositionUpdate();
        LerpPosition();
    }

    /// <summary>
    /// 二点間を補間する
    /// </summary>
    void LerpPosition()
    {
        if (!isServer)
        {
            if (syncPosition == Vector3.zero) return;
            myTransform.position = Vector3.Lerp(myTransform.position, syncPosition, Time.deltaTime * lerpRate);
        }
    }

    void PositionUpdate()
    {
        if (isServer)
        {
            syncPosition = transform.position;
        }
    }
}

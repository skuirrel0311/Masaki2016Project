using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AnimationSync : NetworkBehaviour
{

    Animator animator;

    float oldWeight;

    [SyncVar]
    float weight;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        weight = 0;
        oldWeight = weight;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if ((animator.GetLayerWeight(1) != oldWeight) && isLocalPlayer)
        {
            CmdSetWeight(animator.GetLayerWeight(1));
        }

        oldWeight = weight;
        animator.SetLayerWeight(1,weight);
    }

    [ClientCallback]
    [Command]
    void CmdSetWeight(float w)
    {
        weight = w;
    }
}

using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class PlayerDamage : NetworkBehaviour
{
    [SerializeField]
    GameObject HitEffect;
    private PlayerState state;

    GameObject camera;

    [SerializeField]
    float InpulusPower = 1000;

    public static bool isInpuls = false;

    void Start()
    {
        state = gameObject.GetComponent<PlayerState>();
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        isInpuls = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ammo")
        {
            if (isLocalPlayer && !col.gameObject.GetComponent<Shot>().isLocal)
            {
                CmdHitEffect(col.gameObject.transform.position);
                state.animator.CrossFadeInFixedTime("damage", 0.1f);
                Vector3 vec = col.gameObject.GetComponent<Rigidbody>().velocity;
                vec = new Vector3(vec.x, 0, vec.z);
                vec.Normalize();
                gameObject.GetComponent<Rigidbody>().AddForce(vec * InpulusPower, ForceMode.Impulse);
                isInpuls = true;
                Destroy(col.gameObject);
            }
            if (!isLocalPlayer && col.gameObject.GetComponent<Shot>().isLocal)
            {
                Destroy(col.gameObject);
            }
        }
    }
    [Command]
    void CmdHitEffect(Vector3 position)
    {
        Debug.Log("HitEffect");
        GameObject go = Instantiate(HitEffect, position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(go);
    }
}

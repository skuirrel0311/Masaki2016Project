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

    NoiseAndGrain noise;

    void Start()
    {
        state = gameObject.GetComponent<PlayerState>();
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        noise = camera.GetComponent<NoiseAndGrain>();
    }
    void Update()
    {
        if (isLocalPlayer)
        {
            noise.intensityMultiplier = ((float)(state.maxHp - state.Hp) / (float)state.maxHp) * 10;
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ammo")
        {
            if (isLocalPlayer&&!col.gameObject.GetComponent<Shot>().isLocal)
            {
                CmdHitEffect(col.gameObject.transform.position);
                state.animator.CrossFadeInFixedTime("damage", 0.1f);
                state.Damege();
                Destroy(col.gameObject);
            }
            else
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

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

    [SerializeField]
    float InpulusPower = 100;

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
                Vector3 vec = col.gameObject.GetComponent<Rigidbody>().velocity;
                vec = new Vector3(vec.x,vec.y/100,vec.z);
                vec.Normalize();
                gameObject.GetComponent<Rigidbody>().AddForce(vec*InpulusPower,ForceMode.Impulse);
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

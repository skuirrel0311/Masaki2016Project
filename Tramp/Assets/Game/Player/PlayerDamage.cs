using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerDamage : NetworkBehaviour
{
    [SerializeField]
    GameObject HitEffect;
    private PlayerState state;

    void Start()
    {
        state = gameObject.GetComponent<PlayerState>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (!isLocalPlayer) return;
        if (col.gameObject.tag == "Ammo")
        {
            Debug.Log("Hit");
                CmdHitEffect(col.gameObject.transform.position);
                state.Damege();
            
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

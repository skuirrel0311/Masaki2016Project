using UnityEngine;
using System.Collections;

public class PlayerShot : MonoBehaviour
{

    [SerializeField]
    GameObject Ammo;

    [SerializeField]
    Transform shotPosition;

    void Update()
    {
        Shot();
    }

    void Shot()
    {
        if (!Input.GetButtonDown("Fire2")) return;

        Instantiate(Ammo, shotPosition.position, shotPosition.rotation);
    }
}

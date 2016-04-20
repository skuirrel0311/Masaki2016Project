using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GamepadInput;

public class PlayerShot : NetworkBehaviour
{

    [SerializeField]
    GameObject Ammo;

    GameObject cameraObj;
    Camera cam;

    private PlayerState playerState;
    private int playerNum;

    /// <summary>
    /// 弾の発射される位置
    /// </summary>
    [SerializeField]
    private Transform shotPosition;

    /// <summary>
    /// 弾の発射間隔
    /// </summary>
    [SerializeField]
    float shotDistance = 0.2f;

    void Start()
    {
        playerNum = GetComponent<PlayerControl>().playerNum;
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("Camera" + playerNum);
        cam = cameraObj.GetComponentInChildren<Camera>();
        StartCoroutine("LongTriggerDown");
    }
    void Shot()
    {
        //カメラの中心座標からレイを飛ばす
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hit, 100))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = ray.origin + (ray.direction * 100);
        }

        //先にプレイヤーをカメラと同じ方向に向ける
        Quaternion cameraRotation = cameraObj.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        transform.rotation = cameraRotation;
        shotPosition.LookAt(targetPosition);
        if (isServer)
        {
            GameObject go = Instantiate(Ammo, shotPosition.position, shotPosition.rotation) as GameObject;
            NetworkServer.Spawn(go);
        }
        else
        {
            GameObject go = Instantiate(Ammo,shotPosition.position, shotPosition.rotation) as GameObject;
            CmdAmmoSpawn(shotPosition.position, shotPosition.rotation);
        }

    }

    [Command]
    public void CmdAmmoSpawn(Vector3 shotposition,Quaternion shotrotation)
    {
        GameObject go = Instantiate(Ammo, shotposition,shotrotation) as GameObject;
        go.transform.rotation = shotrotation;
    }

    IEnumerator LongTriggerDown()
    {
        while (true)
        {
     if (GamePad.GetTrigger(GamePad.Trigger.RightTrigger, (GamePad.Index)playerNum, true) > 0 && !playerState.IsAppealing&&isLocalPlayer)
                Shot();
            yield return new WaitForSeconds(shotDistance);
        }
    }
}

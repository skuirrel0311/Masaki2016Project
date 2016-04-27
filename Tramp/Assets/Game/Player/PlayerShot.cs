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

    /// <summary>
    /// 弾の残弾数
    /// </summary>
    [SerializeField]
    int stockMax = 30;
    [SerializeField]
    int stock;

    /// <summary>
    /// リロードする必要があるか？
    /// </summary>
    bool IsReload;

    void Start()
    {
        playerNum = GetComponent<PlayerControl>().playerNum;
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("Camera" + playerNum);
        cam = cameraObj.GetComponentInChildren<Camera>();
        StartCoroutine("LongTriggerDown");
        stock = stockMax;
        IsReload = false;
    }

    void Shot()
    {
        //ストックを減らす
        stock--;

        //カメラの中心座標からレイを飛ばす
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPosition = Vector3.zero;

        //先にプレイヤーをカメラと同じ方向に向ける
        Quaternion cameraRotation = cameraObj.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        transform.rotation = cameraRotation;

        if(Physics.Raycast(ray,out hit,100))
        {
            //衝突点がカメラとプレイヤーの間にあるか判定
            Vector3 temp = hit.point - shotPosition.position;
            float angle = Vector3.Angle(ray.direction,temp);

            if(angle < 90) targetPosition = hit.point;                  //９０度以内
            else targetPosition = ray.origin + (ray.direction * 100);   //角度がありすぎる
        }
        else
        {
            targetPosition = ray.origin + (ray.direction * 100);
        }

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
            if (IsReload) yield return new WaitForSeconds(3.5f);

            if(IsReload)
            {
                //リロードが終わったら
                IsReload = false;
                gameObject.GetComponentInChildren<Animator>().SetBool("Reload", false);
                GetComponent<PlayerControl>().enabled = true;
                stock = stockMax;
            }

            if (GamePad.GetTrigger(GamePad.Trigger.RightTrigger, (GamePad.Index)playerNum, true) > 0 && !playerState.IsAppealing && isLocalPlayer)
                Shot();
            else
                yield return null;

            //ストックが無くなった
            if (stock < 0)
            {
                IsReload = true;
                gameObject.GetComponentInChildren<Animator>().SetBool("Reload",true);
                //動けない
                GetComponent<PlayerControl>().enabled = false;
            }

            yield return new WaitForSeconds(shotDistance);
        }
    }
}

/*PlayerShot.cs すべて */

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

    /// <summary>
    /// 弾を自動で補正する範囲
    /// </summary>
    [SerializeField]
    float playerLodkonAngle = 10;

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

        //ターゲットを取得
        Vector3 targetPosition = GetTargetPosition();

        Vector2 vec = new Vector2(targetPosition.x - transform.position.x, targetPosition.z - transform.position.z);
        float rotationY = Mathf.Atan2(vec.x, vec.y) * Mathf.Rad2Deg;

        //ターゲットのほうを向く
        transform.rotation = Quaternion.Euler(0, rotationY, 0);

        //弾の方向ベクトルを決定する
        shotPosition.LookAt(targetPosition);

        if (isServer)
        {
            GameObject go = Instantiate(Ammo, shotPosition.position, shotPosition.rotation) as GameObject;
            NetworkServer.Spawn(go);
        }
        else
        {
            GameObject go = Instantiate(Ammo, shotPosition.position, shotPosition.rotation) as GameObject;
            CmdAmmoSpawn(shotPosition.position, shotPosition.rotation);
        }

    }

    /// <summary>
    /// 弾を撃つ時の目的地を返す
    /// </summary>
    Vector3 GetTargetPosition()
    {
        //プレイヤーが見えていたらプレイヤーの座標がターゲット
        if (LookPlayer()) return GetAdversary().transform.position + Vector3.up;

        //カメラの中心座標からレイを飛ばす
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hit, 100))
        {
            //衝突点がカメラとプレイヤーの間にあるか判定
            Vector3 temp = hit.point - shotPosition.position;
            float angle = Vector3.Angle(ray.direction, temp);

            if (angle < 90) targetPosition = hit.point;                  //９０度以内
            else targetPosition = ray.origin + (ray.direction * 100);   //角度がありすぎる
        }
        else
        {
            targetPosition = ray.origin + (ray.direction * 100);
        }

        return targetPosition;
    }

    /// <summary>
    /// プレイヤーが見えるか？
    /// </summary>
    /// <returns></returns>
    private bool LookPlayer()
    {
        GameObject obj = GetAdversary();
        if (obj == null) return false;
        Vector2 toPlayerVector = new Vector2(obj.transform.position.x - transform.position.x,
            obj.transform.position.z - transform.position.z);
        Vector2 cameraForward = new Vector2(cameraObj.transform.forward.x, cameraObj.transform.forward.z);

        float angle = Vector2.Angle(toPlayerVector, cameraForward);

        return angle < 10;
    }

    /// <summary>
    /// 対戦プレイヤーを取得します
    /// </summary>
    GameObject GetAdversary()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in players)
        {
            if (obj.Equals(gameObject)) continue;
            return obj;
        }
        return null;
    }

    [Command]
    public void CmdAmmoSpawn(Vector3 shotposition, Quaternion shotrotation)
    {
        GameObject go = Instantiate(Ammo, shotposition, shotrotation) as GameObject;
        go.transform.rotation = shotrotation;
    }

    IEnumerator LongTriggerDown()
    {
        while (true)
        {
            if (IsReload)
            {
                while (true)
                {
                    //xを押すまで抜けられない
                    if (!GamePad.GetButtonDown(GamePad.Button.X, (GamePad.Index)playerNum)) yield return null;
                    else break;
                }
                //ボタン押したら3秒待つ
                gameObject.GetComponentInChildren<Animator>().SetBool("Reload", true);
                //動けない
                GetComponent<PlayerControl>().enabled = false;
                yield return new WaitForSeconds(3);

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
            }

            yield return new WaitForSeconds(shotDistance);
        }
    }

    void OnGUI()
    {
        if (!IsReload) return;

        GUI.TextField(new Rect(500, 100, 100, 30), "Push_X Reload");
    }

}
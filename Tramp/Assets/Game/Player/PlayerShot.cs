using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GamepadInput;
using XInputDotNetPure;

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
    //弾を連射中か
    bool isShot;

    /// <summary>
    /// リロードする必要があるか？
    /// </summary>
    bool IsReload;

    GamepadInputState padState;

    void Start()
    {
        playerNum = GetComponent<PlayerControl>().playerNum;
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("Camera" + playerNum);
        cam = cameraObj.GetComponentInChildren<Camera>();
        StartCoroutine("LongTriggerDown");
        stock = stockMax;
        IsReload = false;
        isShot = false;
        vibrationTimer = 0;
    }

    float vibrationTimer;

    void Shot()
    {

        playerState.animator.SetLayerWeight(1, 1);

        GamePadState padState = GamePad.GetState(PlayerIndex.One);

        if (vibrationTimer == -1)
        {
            GamePad.SetVibration(PlayerIndex.One, 0.2f, 0.2f);
            vibrationTimer = 0;
        }


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

        if (!isShot)
            transform.rotation = cameraRotation;

        isShot = true;

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

        shotPosition.LookAt(targetPosition);

        if (isServer)
        {
            GameObject go = Instantiate(Ammo, shotPosition.position, shotPosition.rotation) as GameObject;
            go.GetComponent<Shot>().isLocal=isLocalPlayer;
            NetworkServer.Spawn(go);
        }
        else
        {
            GameObject go = Instantiate(Ammo, shotPosition.position, shotPosition.rotation) as GameObject;
            go.GetComponent<Shot>().isLocal = isLocalPlayer;
            CmdAmmoSpawn(shotPosition.position, shotPosition.rotation);
        }
    }

    void Update()
    {
        if (GamePadInput.GetTrigger(GamePadInput.Trigger.RightTrigger, (GamePadInput.Index)playerNum, true) <= 0 && isLocalPlayer)
        {
            playerState.animator.SetBool("RunShotEnd", true);
            isShot = false;
        }

        if (vibrationTimer != -1)
        {
            vibrationTimer += Time.deltaTime;
            if (vibrationTimer > 0.05f)
            {
                vibrationTimer = -1;
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }
        }
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
                    if (!GamePadInput.GetButtonDown(GamePadInput.Button.X, (GamePadInput.Index)playerNum)) yield return null;
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

            if (GamePadInput.GetTrigger(GamePadInput.Trigger.RightTrigger, (GamePadInput.Index)playerNum, true) > 0 && !playerState.IsAppealing && isLocalPlayer)
            {
                Shot();

                playerState.animator.SetBool("RunShotEnd", false);
            }
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
    public float testnum=0;
    void OnAnimatorIK(int layerIndex)
    {
        if (isShot)
        {

            Vector3 localAngles = playerState.animator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;

            //先にプレイヤーをカメラと同じ方向に向ける
            Vector3 cameraRotation = cameraObj.transform.eulerAngles;

            cameraRotation=cameraRotation - transform.rotation.eulerAngles;

            playerState.animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(cameraRotation.y, 180.0f,-cameraRotation.x+180.0f));
        }

        else
        {
            playerState.animator.SetLayerWeight(layerIndex, 0);
        }
    }

    void OnGUI()
    {
        if (!IsReload) return;

        GUI.TextField(new Rect(500, 100, 100, 30), "Push_X Reload");
    }

}

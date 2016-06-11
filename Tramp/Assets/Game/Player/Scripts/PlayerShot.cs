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
    /// 弾を発射するのに必要なエネルギーの残量の最大値
    /// </summary>
    [SerializeField]
    int stockMax = 500;
    [SerializeField]
    int stock; //現在のエネルギー残量
    public int StockMax { get { return stockMax; } }
    //弾を発射するのに必要なエネルギー量
    [SerializeField]
    int shotEnergyNum = 30;
    int anchorEnergy = 20;
    public int Stock { get { return stock; } }

    //弾を連射中か
    bool isShot;

    /// <summary>
    /// リロードする必要があるか？
    /// </summary>
    bool IsReload;
    bool IsReloading;

    float shotTimer;
    float animationTimer;

    GamepadInputState padState;

    void Start()
    {
        playerNum = GetComponent<PlayerControl>().playerNum;
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("Camera" + playerNum);
        cam = cameraObj.GetComponentInChildren<Camera>();
        stock = stockMax;
        IsReload = false;
        isShot = false;
        shotTimer = -1;
        vibrationTimer = 0;
        animationTimer = -1;
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
        
        Vector3 targetPosition = GetTargetPosition();

        //先にプレイヤーをカメラと同じ方向に向ける
        Quaternion cameraRotation = cameraObj.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        if (!isShot)
            transform.rotation = cameraRotation;

        isShot = true;

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
            animationTimer = -1;
            isShot = false;
        }

        if (GamePadInput.GetTrigger(GamePadInput.Trigger.RightTrigger, GamePadInput.Index.One, true) > 0 && isLocalPlayer && shotTimer == -1 && !isShot)
        {
            shotTimer = 0;
            Shot();
            if(animationTimer==-1)
            {
                animationTimer = 0;
            }
        }

        if (shotTimer != -1)
        {
            shotTimer += Time.deltaTime;
            if (shotTimer >= 0.1f)
            {
                shotTimer = -1;
            }
        }

        if(animationTimer!=-1)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= 0.3f)
            {
                playerState.animator.SetLayerWeight(1, 0);

            }
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


    Vector3 GetTargetPosition()
    {
        if (LookPlayer()) return GetAdversary().transform.position + Vector3.up;

        //カメラの中心座標からレイを飛ばす
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {

            Vector3 temp = hit.point - shotPosition.position;
            float angle = Vector3.Angle(ray.direction, temp);

            if (angle < 90) return hit.point;
            else return ray.origin + (ray.direction * 100);
        }
        else return ray.origin + (ray.direction * 100);
    }

    /// <summary>
    /// 対戦相手が見えるかどうか？
    /// </summary>
    bool LookPlayer()
    {
        GameObject obj = GetAdversary();

        if (obj == null) return false;

        Vector3 toPlayerVector = obj.transform.position - cameraObj.transform.position;
        Vector3 cameraForward = cameraObj.transform.forward;

        float verticalAngle = VerticalAngle(toPlayerVector, cameraObj.transform.forward);
        verticalAngle = Mathf.Abs(verticalAngle);
        float horizontalAngle = Vector2.Angle(new Vector2(toPlayerVector.x, toPlayerVector.z),
            new Vector2(cameraForward.x, cameraForward.z));

        return verticalAngle < 5 && horizontalAngle < 5;
    }

    /// <summary>
    /// 縦方向のAngleを返す
    /// </summary>
    float VerticalAngle(Vector3 vec1, Vector3 vec2)
    {
        //X方向だけのベクトルに変換
        Vector3 temp1 = Vector3.right * vec1.magnitude;
        Vector3 temp2 = Vector3.right * vec2.magnitude;

        //Y座標を代入
        temp1.y = vec1.y;
        temp2.y = vec2.y;

        //それぞれの角度を求める
        float rot1 = Mathf.Atan2(temp1.y, temp1.x) * Mathf.Rad2Deg;
        float rot2 = Mathf.Atan2(temp2.y, temp2.x) * Mathf.Rad2Deg;

        return rot1 - rot2;
    }

    /// <summary>
    /// 対戦相手を取得します
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

    public float testnum=0;
    void OnAnimatorIK(int layerIndex)
    {
        if (isShot)
        {
            Vector3 localAngles = playerState.animator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;

            //先にプレイヤーをカメラと同じ方向に向ける
            Vector3 cameraRotation = cameraObj.transform.eulerAngles;

            cameraRotation=cameraRotation - transform.rotation.eulerAngles;
            if (isServer)
            {
                playerState.animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(cameraRotation.y, 180.0f, -cameraRotation.x + 180.0f));
            }
            else
            {
                playerState.animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(-cameraRotation.y, 0, cameraRotation.x));
            }
        }

        else
        {
            playerState.animator.SetLayerWeight(layerIndex, 0);
        }
    }

    void OnGUI()
    {
        if (IsReload) GUI.TextField(new Rect(500, 100, 200, 25), "Push_X Reload or Touch Anchor");

        if (IsReloading) GUI.TextField(new Rect(500, 100, 110, 25), "Now Reloading…");
    }

    public void AnchorHit()
    {
        int temp = stock + anchorEnergy;
        stock =  temp > stockMax ? stockMax : stock += anchorEnergy;
    }

    void OnDestroy()
    {
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
    }

}

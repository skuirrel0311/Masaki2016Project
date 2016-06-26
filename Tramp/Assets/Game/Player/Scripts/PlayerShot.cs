using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using GamepadInput;
using XInputDotNetPure;

public class PlayerShot : NetworkBehaviour
{
    [SerializeField]
    bool assist = false; //アシストを受けるか？

    [SerializeField]
    GameObject Ammo;

    [SerializeField]
    GameObject AmmoClient;

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

    //相手プレイヤー(localPlayerではない)
    public GameObject adversary;
    [SerializeField]
    private Material apparentMaterial; //はっきり見えるマテリアル
    private Material[] defaultMaterial; //もともとついてるマテリアル
    public Image playerName;

    //弾を連射中か
    bool isShot;

    bool isIK;

    [SerializeField]
    AudioClip shotSE;
    AudioSource audioSource;

    /// <summary>
    /// リロードする必要があるか？
    /// </summary>
    bool IsReload;
    bool IsReloading;

    float shotTimer;
    float animationTimer;

    GamepadInputState padState;

    SoundManager soundManager;

    Timer imageTimer = new Timer();

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
        isIK = false;

        string name = !isServer ? "playerName1" : "playerName2";
        playerName =  GameObject.Find(name).GetComponent<Image>();
        name = isServer ? "playerName1" : "playerName2";
        GameObject.Find(name).SetActive(false);

        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    float vibrationTimer;

    void Shot()
    {
        playerState.animator.SetLayerWeight(1, 1);
        isIK = true;

        GamePadState padState = GamePad.GetState(PlayerIndex.One);

        if (vibrationTimer == -1)
        {
            GamePad.SetVibration(PlayerIndex.One, 0.2f, 0.2f);
            vibrationTimer = 0;
        }
        
        Vector3 targetPosition = GetTargetPosition();

        //先にプレイヤーをカメラと同じ方向に向ける
        Quaternion cameraRotation = cam.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        if (!isShot)
            transform.rotation = cameraRotation;

        isShot = true;

        shotPosition.LookAt(targetPosition);
        audioSource.PlayOneShot(shotSE);
        if (isServer)
        {
            GameObject go = Instantiate(Ammo, shotPosition.position, shotPosition.rotation) as GameObject;
            go.GetComponent<Shot>().isLocal=isLocalPlayer;
            NetworkServer.Spawn(go);
        }
        else
        {
            GameObject go = Instantiate(AmmoClient, shotPosition.position, shotPosition.rotation) as GameObject;
            go.GetComponent<Shot>().isLocal = isLocalPlayer;
            CmdAmmoSpawn(shotPosition.position, shotPosition.rotation);
        }
    }

    void Update()
    {
        if (adversary == null) adversary = GetAdversary();

        ShowNameText();

        ChangeAdversaryPlayer();

        SetMarker();

        if (GamePadInput.GetTrigger(GamePadInput.Trigger.RightTrigger, (GamePadInput.Index)playerNum, true) <= 0 && isLocalPlayer)
        {
            playerState.animator.SetBool("RunShotEnd", true);
            animationTimer = -1;
            isShot = false;
        }

        if (GamePadInput.GetTrigger(GamePadInput.Trigger.RightTrigger, GamePadInput.Index.One, true) > 0 && isLocalPlayer && shotTimer == -1 && !isShot&&MainGameManager.isGameStart&&!SoundManager.isEnd)
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
                isIK = false;
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
        GameObject go = Instantiate(AmmoClient, shotposition, shotrotation) as GameObject;
        go.transform.rotation = shotrotation;
    }


    Vector3 GetTargetPosition()
    {
        if (assist && LookPlayer(5))
        {
            return adversary.transform.position + Vector3.up;
        }

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
    bool LookPlayer(int range)
    {
        if (adversary == null) return false;

        Vector3 toPlayerVector = adversary.transform.position - cameraObj.transform.position;
        Vector3 cameraForward = cameraObj.transform.forward;

        float verticalAngle = VerticalAngle(toPlayerVector, cameraObj.transform.forward);
        verticalAngle = Mathf.Abs(verticalAngle);
        float horizontalAngle = Vector2.Angle(new Vector2(toPlayerVector.x, toPlayerVector.z),
            new Vector2(cameraForward.x, cameraForward.z));

        return verticalAngle < range && horizontalAngle < range;
    }

    //指定した距離より近いか？
    bool NearPlayer(float distance)
    {
        if (adversary == null) return false;
        
        return Vector3.Distance(transform.position,adversary.transform.position) < distance;
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
        return GameObject.FindGameObjectsWithTag("Player").ToList().Find(n => !n.Equals(gameObject));
    }

    public float testnum=0;
    void OnAnimatorIK(int layerIndex)
    {
        if (isShot&&isIK)
        {
            Vector3 localAngles = playerState.animator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;

            //先にプレイヤーをカメラと同じ方向に向ける
            Vector3 cameraRotation = cam.transform.eulerAngles;

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
            isIK = false;
        }
    }

    void ShowNameText()
    {
        if (adversary == null) return;
        if (!adversary.GetComponentInChildren<IsRendered>().WasRendered)
        {
            playerName.gameObject.SetActive(false);
            return;
        }

        //相手のプレイヤーが見えていたら
        playerName.gameObject.SetActive(true);
        //x(0～1),y(0～1)
        Vector3 textPosition = cam.WorldToViewportPoint(adversary.transform.position + (Vector3.up * 2));
        textPosition.x = (textPosition.x * 1280) - (1280 * 0.5f);
        textPosition.y = (textPosition.y * 720) - (720 * 0.5f);

        playerName.GetComponent<RectTransform>().anchoredPosition = textPosition;
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

    void ChangeAdversaryPlayer()
    {
        if(adversary == null)return;

        if (LookPlayer(10) && !NearPlayer(13))
        {
            //視界(指定の範囲)に入った瞬間
            if (defaultMaterial == null) defaultMaterial = adversary.GetComponentInChildren<SkinnedMeshRenderer>().materials;
            adversary.GetComponentInChildren<SkinnedMeshRenderer>().material = apparentMaterial;
        }
        else
        {
            if (defaultMaterial != null) adversary.GetComponentInChildren<SkinnedMeshRenderer>().materials = defaultMaterial;
            defaultMaterial = null;
        }
    }

    void SetMarker()
    {
        imageTimer.Update();
        if(assist && LookPlayer(5))
        {
            imageTimer.Stop();
            cameraObj.GetComponent<CameraLockon>().SetMaker(adversary,imageTimer);
        }
        else
        {
            if (!imageTimer.IsWorking) imageTimer.TimerStart(0.3f);
            cameraObj.GetComponent<CameraLockon>().SetMaker(null,imageTimer);
        }
    }
}

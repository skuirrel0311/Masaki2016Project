using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using GamepadInput;


public class CameraControl : MonoBehaviour
{

    #region Field
    /// <summary>
    /// 緯度
    /// </summary>
    [SerializeField]
    float latitude = 15;
    /// <summary>
    /// 経度
    /// </summary>
    [SerializeField]
    float longitude = 180;
    /// <summary>
    /// 球体の半径(ターゲットの位置からの距離)
    /// </summary>
    [SerializeField]
    float radius = 6;
    [SerializeField]
    private float rotationSpeed = 200;

    private GameObject cameraObj;
    private CameraLockon lockon;

    private Vector3 cameraTargetPosition;
    private GameObject player;
    private PlayerControl playerControl;
    private Vector3 oldPlayerPosition;
    private int playerNum = 1;

    //着地したときに戻すlatitudeの値
    public float atJumpLatitude = 15;

    public bool IsEndFallingCamera = true;

    [SerializeField]
    private bool IsControled = false;

    //カメラとプレイヤーの間にあるオブジェクト
    List<GameObject> lineHitObjects = new List<GameObject>();
    #endregion

    void Start()
    {
        cameraObj = transform.FindChild("ThirdPersonCamera").gameObject;
        lockon = gameObject.GetComponent<CameraLockon>();
    }

    //カメラの角度をリセットする
    public void Reset()
    {
        float playerRotation = player.transform.eulerAngles.y;

        //カメラが向きたい方向
        float cameraRotation = playerRotation + 180;

        longitude = cameraRotation;
        latitude = 15;
    }

    public void SetPlayer(GameObject Player)
    {
        player = Player;
        playerControl = player.GetComponent<PlayerControl>();
        oldPlayerPosition = player.transform.position;
        cameraTargetPosition = player.transform.position;
        playerNum = player.GetComponent<PlayerControl>().playerNum;
        Reset();
        lockon.SetPlayer(player);
    }

    void Update()
    {
        if (player == null) return;
        BetweenPlayerAndCamera();

        if (lockon.IsLockOn) return;

        lockon.AlignmentImage(1);
        Vector2 rightStick = GamePadInput.GetAxis(GamePadInput.Axis.RightStick, (GamePadInput.Index)playerNum);

        if (latitude < 0) latitude += -rightStick.y * (rotationSpeed * 2.5f) * Time.deltaTime;
        else latitude += -rightStick.y * rotationSpeed * Time.deltaTime;

        longitude += rightStick.x * rotationSpeed * Time.deltaTime;

        if (playerControl.IsFalling || playerControl.IsFallAfter)
        {
            if (Mathf.Abs(rightStick.y) > 0.3f) IsControled = true;
        }

        if (player.GetComponent<PlayerControl>().IsFalling) FallingCamera();
        else IsControled = false;

        if (GamePadInput.GetButtonDown(GamePadInput.Button.RightStick, (GamePadInput.Index)playerNum)) Reset();

        GetTargetPosition();
        SphereCameraControl();

        cameraObj.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, cameraObj.transform.localRotation.y, 0), cameraObj.transform.localRotation, 1 - lockon.LockonTimer.Progress);

        oldPlayerPosition = player.transform.position;
    }

    /// <summary>
    /// プレイヤーとカメラの間にオブジェクトがあったら非表示にします
    /// </summary>
    void BetweenPlayerAndCamera()
    {
        Vector3 direction = (player.transform.position + Vector3.up) - cameraObj.transform.position;
        Ray ray = new Ray(cameraObj.transform.position, direction);


        //rayにあたったオブジェクトをリストに格納
        List<GameObject> hitList = Physics.RaycastAll(ray, direction.magnitude).Select(n => n.transform.gameObject).ToList();

        if (hitList.Count == 0) return;

        //containsでlinehitに無くてtagがBoxのものを判定しwhereで無かったものをlistに格納
        lineHitObjects.AddRange(hitList.Where(n => (!lineHitObjects.Contains(n)) && n.tag == "Box"));

        //半透明にする
        lineHitObjects.ForEach(n => SetAlpha(n, 0.3f));

        //今回ヒットしなかったものは透明度をリセットし、リムーブする。
        lineHitObjects.RemoveAll(n =>
        {
            if (hitList.Contains(n)) return false;
            ResetAlpha(n);
            return true;
        });
    }

    void SetAlpha(GameObject obj, float alpha)
    {
        Material mat = obj.GetComponent<Renderer>().material;
        Color color = mat.color;
        mat.SetFloat("_Mode", 2);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        mat.color = new Color(color.r, color.g, color.b, alpha);
    }

    void ResetAlpha(GameObject obj)
    {
        Material mat = obj.GetComponent<Renderer>().material;
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }

    /// <summary>
    /// 球体座標系によるカメラの制御
    /// </summary>
    void SphereCameraControl()
    {
        Vector3 cameraPosition;

        //プレイヤーの足元からY座標に+1した座標をターゲットにする
        Vector3 target = cameraTargetPosition + Vector3.up;

        //経度には制限を掛ける
        latitude = Mathf.Clamp(latitude, -120, 60);

        if (latitude < 0)
        {
            //リープ開始
            Vector3 vec1 = SphereCoordinate(longitude, 0);
            //リープ終了時の座標
            Vector3 vec2 = SphereCoordinate(longitude, -17);
            Vector3 toPlayerVector = (Vector3.down - vec2).normalized;
            vec2 += toPlayerVector * 5;

            //latitudeが-120だったらtは1になる
            float t = (-1 * (latitude + 0)) / 120;
            cameraPosition = Vector3.Slerp(vec1, vec2, t);
            transform.position = target + cameraPosition;
        }
        else
        {
            //カメラが地面にめり込まない場合は球体座標をそのまま使う
            cameraPosition = SphereCoordinate(longitude, latitude);
            transform.position = target + cameraPosition;
        }

        transform.LookAt(target);
    }

    /// <summary>
    /// 指定した角度の球体座標を返します
    /// </summary>
    /// <param name="longitude">経度</param>
    /// <param name="latitude">緯度</param>
    /// <returns></returns>
    public Vector3 SphereCoordinate(float longitude, float latitude)
    {
        Vector3 temp = Vector3.zero;

        //重複した計算
        float deg2Rad = Mathf.Deg2Rad;
        float t = radius * Mathf.Cos(latitude * deg2Rad);

        temp.x = t * Mathf.Sin(longitude * deg2Rad);
        temp.y = radius * Mathf.Sin(latitude * deg2Rad);
        temp.z = t * Mathf.Cos(longitude * deg2Rad);

        return temp;
    }

    /// <summary>
    /// ロックオンされたアンカーに合わせたカメラの座標を返します
    /// </summary>
    public Vector3 PositionForLockOnAnchor(GameObject anchor)
    {
        //横方向
        Vector2 vec = new Vector2(anchor.transform.position.x - transform.position.x,
            anchor.transform.position.z - transform.position.z);

        //縦方向
        Vector3 toAnchorVector = lockon.targetAnchor.transform.position - cameraObj.transform.position;
        float rot3 = -DifferenceLatitude(toAnchorVector, Vector3.right);

        float rot1 = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        float rot2 = Mathf.Atan2(transform.forward.z, transform.forward.x) * Mathf.Rad2Deg;
        return SphereCoordinate(longitude + (rot2 - rot1), rot3);
    }
    //落ちているときは下方を見る
    private void FallingCamera()
    {
        //カメラワーク中に操作されたら終了する
        if (IsControled) return;
        if (IsEndFallingCamera) return;

        //地面が近かったらやめる
        if (!IsFarGround()) return;

        if (latitude >= 60)
        {
            IsEndFallingCamera = true;
            return;
        }

        IsEndFallingCamera = false;
        //目的のlatitude
        float a = 60;
        float t = (360 * Time.deltaTime) / (a - latitude);
        latitude = Mathf.Lerp(latitude, a, t);
    }

    //着地予想をして遠かったらtrueを返す
    private bool IsFarGround()
    {
        Vector3 movement = player.transform.position - oldPlayerPosition;
        Ray ray = new Ray(player.transform.position, movement);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, 1000))
        {
            if (hit.distance > 5)   return true;
            else                    return false;
        }

        return false;
    }

    private void GetTargetPosition()
    {
        Vector3 movement = player.transform.position - oldPlayerPosition;

        //落ちていないときに流れていなかったら
        if (!playerControl.IsFalling && !playerControl.IsFlowing) movement.y *= 0.3f;

        cameraTargetPosition += movement;
        
        if (playerControl.IsOnGround && !playerControl.OnGroundTimer.IsLimitTime)
        {
            cameraTargetPosition = Vector3.Lerp(cameraTargetPosition, player.transform.position, playerControl.OnGroundTimer.Progress);
        }

        //カメラワーク中に操作されたら終了する
        if (IsControled) return;

        //落下する前のlatitudeに戻す
        if (playerControl.IsFallAfter && !playerControl.LandedTimer.IsLimitTime)
        {
            latitude = Mathf.Lerp(latitude, atJumpLatitude, playerControl.LandedTimer.Progress);
        }
        else
        {
            playerControl.IsFallAfter = false;
            IsControled = false;
        }
    }

    public void SetNowLatitude()
    {
        atJumpLatitude = latitude;
    }

    /// <summary>
    /// 2つのベクトルの緯度の差を返す
    /// </summary>
    public float DifferenceLatitude(Vector3 vec1, Vector3 vec2)
    {
        float len1 = vec1.magnitude;
        float len2 = vec2.magnitude;

        //X方向だけのベクトルに変換
        Vector3 temp1 = Vector3.right * len1;
        Vector3 temp2 = Vector3.right * len2;

        //Y座標を代入
        temp1.y = vec1.y;
        temp2.y = vec2.y;

        //それぞれの角度を求める
        float rot1 = Mathf.Atan2(temp1.y, temp1.x) * Mathf.Rad2Deg;
        float rot2 = Mathf.Atan2(temp2.y, temp2.x) * Mathf.Rad2Deg;

        return rot1 - rot2;
    }

}
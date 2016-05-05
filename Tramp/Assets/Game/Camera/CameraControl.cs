using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;


public class CameraControl : MonoBehaviour
{

    #region Field
    enum Side
    {
        Right, Left
    }

    public GameObject targetAnchor;

    [SerializeField]
    private float rotationSpeed = 5;

    /// <summary>
    /// 球体の半径(ターゲットの位置からの距離)
    /// </summary>
    [SerializeField]
    float radius = 3;

    [SerializeField]
    private GameObject AlignmentSprite;

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

    private Vector3 oldPlayerPosition;
    public GameObject player;
    private GameObject cameraObj;
    private Vector3 lookatPosition;
    private float timer;
    private bool LockonDecision;

    private int playerNum = 1;
    #endregion

    void Start()
    {
        cameraObj = transform.FindChild("ThirdPersonCamera").gameObject;
        oldPlayerPosition = player.transform.position;
        playerNum = player.GetComponent<PlayerControl>().playerNum;
        LockonDecision = false;
    }

    public void SetPlayer(GameObject Player)
    {
        player = Player;
        oldPlayerPosition = player.transform.position;
        playerNum = player.GetComponent<PlayerControl>().playerNum;
    }

    void Update()
    {
        //ロックオンの処理押された時と押している時で処理を分ける
        if (GamePad.GetButtonDown(GamePad.Button.Y, (GamePad.Index)playerNum))
        {
            CameraLockOnStart();
        }
        if (GamePad.GetButton(GamePad.Button.Y, (GamePad.Index)playerNum) && targetAnchor != null)
        {
            PlayerTrace();
            oldPlayerPosition = player.transform.position;
            AnchorLockOn();
            return;
        }
        LockonDecision = false;

        timer = Mathf.Max(timer - Time.deltaTime, 0);

        Vector2 rightStick = GamePad.GetAxis(GamePad.Axis.RightStick, (GamePad.Index)playerNum);

        if (latitude < 0) latitude += -rightStick.y * (rotationSpeed * 1.5f) * Time.deltaTime;
        else latitude += -rightStick.y * rotationSpeed * Time.deltaTime;
        longitude += rightStick.x * rotationSpeed * Time.deltaTime;

        SphereCameraControl();

        cameraObj.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, cameraObj.transform.localRotation.y, 0), cameraObj.transform.localRotation, timer);

        oldPlayerPosition = player.transform.position;
    }

    /// <summary>
    /// 球体座標系によるカメラの制御
    /// </summary>
    void SphereCameraControl()
    {
        Vector3 cameraPosition;

        //プレイヤーの足元からY座標に+1した座標をターゲットにする
        Vector3 target = player.transform.position;
        target.y += 1f;

        //経度には制限を掛ける
        float temp = Mathf.Clamp(latitude, -50, 60);
        latitude = Mathf.Clamp(latitude, -120, 60);

        if (latitude < 0)
        {
            //カメラが地面にめり込むので球面線形補正をする
            //緯度が0の場合の座標
            Vector3 vec1 = SphereCoordinate(longitude, 0);
            //緯度が-120の場合の座標
            Vector3 vec2 = SphereCoordinate(longitude, -50);
            Vector3 toPlayerVector = (Vector3.zero - vec2).normalized;
            vec2 += toPlayerVector * 2f;

            //latitudeが-120だったらtは1になる
            float t = (latitude * -1) / 120;
            cameraPosition = Vector3.Slerp(vec1, vec2, t);
            transform.position = target + cameraPosition;
        }
        else
        {
            //カメラが地面にめり込まない場合は球体座標をそのまま使う
            cameraPosition = SphereCoordinate(longitude, temp);
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
    Vector3 SphereCoordinate(float longitude, float latitude)
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

    private float VectorToAngle(float a, float b)
    {
        return Mathf.Atan2(a, b) * 180.0f / Mathf.PI;
    }

    /// <summary>
    /// ロックオンをするためのキーを押した時の処理
    /// </summary>
    private void CameraLockOnStart()
    {
        targetAnchor = GetTargetAnchor();
        InitLookatPosition();
        timer = 0;
    }

    private void InitLookatPosition()
    {
        //Camera cam = cameraObj.GetComponent<Camera>();
        //lookatPosition = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, cam.nearClipPlane));

        float len = (targetAnchor.transform.position - transform.position).magnitude;

        lookatPosition = transform.position + (transform.forward * len);

    }

    /// <summary>
    ///指定されたアンカーのある方向に向かってカメラを移動
    /// </summary>
    private void AnchorLockOn()
    {
        timer = Mathf.Min(timer + Time.deltaTime, 1);

        if (timer == 1) LockonDecision = true;
        if (LockonDecision)
            AlignmentImage(1);
        else
            AlignmentImage(timer);

        if (GamePad.GetButtonDown(GamePad.Button.LeftShoulder, (GamePad.Index)playerNum)) targetAnchor = GetSideAnchor(Side.Left);

        if (GamePad.GetButtonDown(GamePad.Button.RightShoulder, (GamePad.Index)playerNum)) targetAnchor = GetSideAnchor(Side.Right);

        //アンカーのある方向を取得
        Vector3 vec = targetAnchor.transform.position - transform.position;
        //オブジェクトのある方向に合わせたカメラのポジション移動
        Vector3 playerPosition = player.transform.position + Vector3.up;
        transform.position = playerPosition + PositionForLockOnAnchor(targetAnchor);
        //カメラの注視点を移動
        cameraObj.transform.LookAt(targetAnchor.transform);
        //cameraObj.transform.LookAt(Vector3.Lerp(lookatPosition, targetAnchor.transform.position, timer));
    }

    /// <summary>
    /// ロックオンされたアンカーに合わせたカメラの座標を返します
    /// </summary>
    Vector3 PositionForLockOnAnchor(GameObject anchor)
    {
        //横方向
        Vector2 vec = new Vector2(anchor.transform.position.x - transform.position.x,
            anchor.transform.position.z - transform.position.z);

        //縦方向
        float rot3 = DifferenceLatitude(targetAnchor.transform.position);

        float rot1 = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        float rot2 = Mathf.Atan2(transform.forward.z, transform.forward.x) * Mathf.Rad2Deg;
        return SphereCoordinate(longitude + (rot2 - rot1), -rot3);
    }

    /// <summary>
    /// 緯度の差を返す
    /// </summary>
    float DifferenceLatitude(Vector3 anchor)
    {
        float len = (anchor - transform.position).magnitude;

        //単一方向に変換して傾きを出す
        //rightは(1,0,0)
        Vector3 vec = Vector3.right * len;

        vec.y = anchor.y;
        //(1,1)だと45となる。
        //Debug.Log("(1,1) = " + Mathf.Atan2(1,1) * Mathf.Rad2Deg);

        return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    ///照準画像の処理
    /// </summary>
    private void AlignmentImage(float timer)
    {
        AlignmentSprite.SetActive(true);
        Image img = AlignmentSprite.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, timer * timer);
        AlignmentSprite.transform.localScale = Vector3.one * 2 * ((1 - timer * timer) + 0.5f);
    }

    /// <summary>
    /// 左右どちらかにある一番近い次のアンカーに移動する
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    private GameObject GetSideAnchor(Side side)
    {
        Vector2 originAnchorVec = new Vector2(targetAnchor.transform.position.x - transform.position.x
                                                                      , targetAnchor.transform.position.z - transform.position.z);//アンカーとの角度を代入する
        float angle;//右あるいは左に最も近いアンカーとの角度を代入する

        angle = 360;

        GameObject nextAnchor = null;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");

        foreach (GameObject obj in objects)
        {
            if (targetAnchor.Equals(obj)) continue;

            Vector2 vec = new Vector2(obj.transform.position.x - transform.position.x
                                                     , obj.transform.position.z - transform.position.z);

            float tmpAngle = Vector2.Angle(vec, originAnchorVec);
            //PQ×PA = PQ.x・PA.y - PQ.y・PA.x ：外積の式
            //外積を使って現在参照しているオブジェクトからオブジェクトが左右どちらにあるか判定する
            float crossProduct = CrossProductToVector2(originAnchorVec, vec);
            bool withinAngle = false;
            //一番角度が小さいオブジェクトを検索する
            if ((side == Side.Right && crossProduct < 0) || side == Side.Left && crossProduct > 0)
                withinAngle = (tmpAngle >= 0 && tmpAngle < angle);

            if (withinAngle)
            {
                angle = tmpAngle;
                nextAnchor = obj;
            }
        }
        if (nextAnchor == null)
        {
            nextAnchor = targetAnchor;
        }

        else
        {
            InitLookatPosition();
            timer = 0;
        }

        return nextAnchor;
    }

    /// <summary>
    /// Vector3を水平のVector2に変換する
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    private Vector2 PlaneVector2ToVector3(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    /// <summary>
    /// 外積を求める
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private float CrossProductToVector2(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    GameObject GetTargetAnchor()
    {
        GameObject targetAnchor = null;
        
        //カメラに写っているアンカーを取得
        List<GameObject> anchorList = GetViewAnchor();
        if (anchorList.Count == 0) return null;

        //見ている可能性が高いアンカーを取得
        List<GameObject> temp = GetShouldLookAnchor(anchorList);

        if(temp.Count != 0)
        {
            //10度以内のアンカーのなかで一番近いアンカーを取得
            targetAnchor = GetNearAnchor(temp);
        }
        else
        {
            //10度以内にアンカーが存在しなかったら一番角度の低いアンカーを取得
            targetAnchor = GetLowAngleAnchor(anchorList);
        }

        return targetAnchor;
    }

    /// <summary>
    /// 渡されたリストの中から最も近いアンカーを返します
    /// </summary>
    GameObject GetNearAnchor(List<GameObject> anchorList)
    {
        GameObject nearAnchor = null;
        float distance = 100000;

        anchorList.ForEach(n =>
        {
            if(Vector3.Distance(transform.position,n.transform.position) < distance)
            {
                distance = Vector3.Distance(transform.position, n.transform.position);
                nearAnchor = n;
            }
        });

        return nearAnchor;
    }

    /// <summary>
    /// カメラに写っているアンカーのリストを返します
    /// </summary>
    List<GameObject> GetViewAnchor()
    {
        List<GameObject> anchorList = new List<GameObject>();
        anchorList.AddRange(GameObject.FindGameObjectsWithTag("Anchor"));

        if (anchorList.Count <= 0) return null;

        return anchorList.FindAll(n => n.GetComponent<IsRendered>().WasRendered);
    }

    /// <summary>
    /// 見ている可能性の高い(10度以内)アンカーをすべて返します
    /// </summary>
    List<GameObject> GetShouldLookAnchor(List<GameObject> anchorList)
    {
        Vector2 originAnchorVec = new Vector2(transform.forward.x, transform.forward.z);
        anchorList = anchorList.FindAll(n =>
        {
            Vector2 vec = new Vector2(n.transform.position.x - transform.position.x
                                         , n.transform.position.z - transform.position.z);

            float tmpAngle = Vector2.Angle(vec, originAnchorVec);

            //10度以内のアンカーを検索
            return tmpAngle < 10;
        });
        
        return anchorList;
    }

    /// <summary>
    /// 一番角度が小さいアンカーを返します
    /// </summary>
    GameObject GetLowAngleAnchor(List<GameObject> anchorList)
    {
        GameObject anchor = null;
        float angle = 360;
        Vector2 originAnchorVec = new Vector2(transform.forward.x, transform.forward.z);

        anchorList.ForEach(n =>
        {
            Vector2 vec = new Vector2(n.transform.position.x - transform.position.x
                             , n.transform.position.z - transform.position.z);

            float tmpAngle = Vector2.Angle(vec, originAnchorVec);

            //一番小さい角度のアンカーを検索
            if (tmpAngle < angle)
            {
                angle = tmpAngle;
                anchor = n;
            }
        });

        return anchor;
    }

    /// <summary>
    /// プレイヤーの移動に合わせてカメラの位置を移動
    /// </summary>
    private void PlayerTrace()
    {
        Vector3 movement = player.transform.position - oldPlayerPosition;

        //プレイヤーが移動していなかったら終了
        if (movement.magnitude == 0) return;

        //プレイヤーについていくMOMO
        transform.position += movement;
    }
}

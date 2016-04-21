using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GamepadInput;


public class CameraControl : MonoBehaviour
{
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

    /// <summary>
    /// 極座標から足す距離(カメラが地面を滑る時に使用します)
    /// </summary>
    float addDistance;

    [SerializeField]
    private GameObject AlignmentSprite;
    
    /// <summary>
    /// 緯度
    /// </summary>
    float latitude = 15;
    /// <summary>
    /// 経度
    /// </summary>
    float longitude = 180;

    private Vector3 oldPlayerPosition;
    public GameObject player;
    private GameObject cameraObj;
    private Vector3 lookatPosition;
    private float timer;
    private bool LockonDecision;

    private int playerNum = 1;

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
        if (GamePad.GetButtonDown(GamePad.Button.Y,(GamePad.Index)playerNum))
        {
            CameraLockOnStart();
        }
        if (GamePad.GetButton(GamePad.Button.Y,(GamePad.Index)playerNum)&& targetAnchor != null)
        {
            PlayerTrace();
            AnchorLockOn();
            return;
        }
        LockonDecision = false;

        timer = Mathf.Max(timer - Time.deltaTime, 0);

        Vector2 rightStick = GamePad.GetAxis(GamePad.Axis.RightStick, (GamePad.Index)playerNum);

        latitude += -rightStick.y * rotationSpeed * Time.deltaTime;
        longitude += rightStick.x * rotationSpeed * Time.deltaTime;

        //経度が-25を超えていたら
        if (latitude <= -25)
        {
            //プレイヤーの方向に行く
            addDistance += rightStick.y * 5 * Time.deltaTime;

            addDistance = Mathf.Clamp(addDistance, 0, 2.5f);
        }
        else addDistance = 0;
        
        SphereCameraControl();

        cameraObj.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, cameraObj.transform.localRotation.y, 0), cameraObj.transform.localRotation, timer);
    }

    /// <summary>
    /// 球体座標系によるカメラの制御
    /// </summary>
    void SphereCameraControl()
    {
        Vector3 cameraPosition;

        float deg2Rad = Mathf.Deg2Rad;

        //経度には制限を掛ける
        float temp = Mathf.Clamp(latitude, -25, 80);

        cameraPosition.x = radius * Mathf.Cos(temp * deg2Rad) * Mathf.Sin(longitude * deg2Rad);
        cameraPosition.y = radius * Mathf.Sin(temp * deg2Rad);
        cameraPosition.z = radius * Mathf.Cos(temp * deg2Rad) * Mathf.Cos(longitude * deg2Rad);

        //プレイヤーの足元からY座標に+1した座標をターゲットにする
        Vector3 target = player.transform.position;
        target.y += 1f;

        transform.position = cameraPosition + target;

        //地面を滑っている場合はその分座標を足す
        if (addDistance != 0)
        {
            Vector3 toPlayerVector = (player.transform.position - transform.position).normalized;
            transform.position += toPlayerVector * addDistance;
        }

        transform.LookAt(target);
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
        targetAnchor = GetNearAnchor();
        InitLookatPosition();
        timer = 0;
    }

    private void InitLookatPosition()
    {
        Camera cam = cameraObj.GetComponent<Camera>();
        lookatPosition = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, cam.nearClipPlane));
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
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, VectorToAngle(vec.x, vec.z), 0), timer);
        //カメラの注視点を移動
        cameraObj.transform.LookAt(Vector3.Lerp(lookatPosition, targetAnchor.transform.position, timer));
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

    //todo:CreateFlowとメソッドがかぶっているのでライブラリを作成する
    GameObject GetNearAnchor()
    {
        GameObject gameObject = null;
        //一番近いアンカーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");
        if (objects.Length <= 0) return null;

        float distance = 1000000;

        foreach (GameObject obj in objects)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) < distance)
            {
                Vector3 vec = obj.transform.position - transform.position;
                distance = vec.magnitude;
                gameObject = obj;
            }
        }

        return gameObject;
    }

    /// <summary>
    /// プレイヤーの移動に合わせてカメラの位置を移動
    /// </summary>
    private void PlayerTrace()
    {
        Vector3 movement = player.transform.position - oldPlayerPosition;

        //プレイヤーが移動していなかったら終了
        if (movement.magnitude == 0) return;

        //プレイヤーについていく
        transform.position += movement;

        oldPlayerPosition = player.transform.position;
    }
}

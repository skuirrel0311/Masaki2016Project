using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class CameraControl : MonoBehaviour
{
    enum Side
    {
        Right, Left
    }

    public GameObject targetAnchor;

    [SerializeField]
    private float rotationSpeed = 5;

    [SerializeField]
    private GameObject AlignmentSprite;
    private float rotationY;

    private Vector3 oldPlayerPosition;
    private GameObject player;
    private GameObject cameraObj;
    private Vector3 lookatPosition;
    private float timer;
    private bool LockonDecision;

    void Start()
    {
        rotationY = 0;
        player = GameObject.Find("Player");
        cameraObj = transform.FindChild("Main Camera").gameObject;
        oldPlayerPosition = player.transform.position;
        LockonDecision = false;
    }

    void Update()
    {
        PlayerTrace();

        //ロックオンの処理押された時と押している時で処理を分ける
        if (Input.GetKeyDown(KeyCode.R))
        {
            CameraLockOnStart();
        }
        if (Input.GetKey(KeyCode.R) && targetAnchor != null)
        {
            AnchorLockOn();
            return;
        }
        timer = Mathf.Max(timer - Time.deltaTime, 0);
        float mouseX = Input.GetAxis("Horizontal2");
        AlignmentSprite.SetActive(false);
        rotationY += mouseX * rotationSpeed;

        cameraObj.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, cameraObj.transform.rotation.y, 0), cameraObj.transform.rotation, timer);
        //オブジェクトのある方向に合わせたカメラのポジション移動
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, rotationY, 0), transform.rotation, timer);
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
        //削除キーが押された場合は対象のアンカーを削除する
        if (Input.GetKeyDown(KeyCode.D))
        {
            Destroy(targetAnchor);
        }

        if (Input.GetKeyDown(KeyCode.E)) targetAnchor = GetSideAnchor(Side.Left);

        if (Input.GetKeyDown(KeyCode.T)) targetAnchor = GetSideAnchor(Side.Right);

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

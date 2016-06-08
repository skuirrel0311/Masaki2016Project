using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GamepadInput;

public class CameraLockon : MonoBehaviour
{
    #region Field
    enum Side { Right, Left }
    CameraControl control;
    private GameObject cameraObj;

    //ロックオンのターゲット
    public GameObject targetAnchor;
    private Vector3 cameraTargetPosition;

    //カメラの所有者(プレイヤー)
    private GameObject player;
    private PlayerControl playerControl;
    private Vector3 oldPlayerPosition;
    private int playerNum = 1;
    private float oldInputVec = 0;

    //ロックオンマーカー
    [SerializeField]
    private GameObject AlignmentSprite = null;
    private RectTransform canvasRect;

    //タイマー
    private Timer lockonTimer = new Timer(); //ロックオンにかかる時間の制御
    public Timer LockonTimer { get { return lockonTimer; } }
    private Timer imageTimer = new Timer();  //マーカーの拡大縮小にかかる時間の制御

    //状態
    private bool IsEndLockOn;       //ロックオンが終わったか？
    public bool IsLockOn;           //ロックオン開始
    #endregion

    void Start()
    {
        control = gameObject.GetComponent<CameraControl>();
        cameraObj = transform.FindChild("ThirdPersonCamera").gameObject;
        IsEndLockOn = false;
        canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        playerControl = player.GetComponent<PlayerControl>();
        oldPlayerPosition = player.transform.position;
        cameraTargetPosition = player.transform.position;
        playerNum = player.GetComponent<PlayerControl>().playerNum;

        lockonTimer.TimerStart(0.2f);
    }

    void Update()
    {
        if (player == null) return;
        UpdateTimer();

        //ロックオンの処理押された時と押している時で処理を分ける
        if (GamePadInput.GetButtonDown(GamePadInput.Button.LeftShoulder, (GamePadInput.Index)playerNum) && !MainGameManager.IsPause)
        {
            if (!IsLockOn) CameraLockOnStart();
            else
            {
                IsLockOn = false;
                lockonTimer.TimerStart(0.2f); //戻る時の速さ
            }
        }
        if (IsLockOn && targetAnchor != null)
        {
            PlayerTrace();
            oldPlayerPosition = player.transform.position;
            AnchorLockOn();
            return;
        }
    }

    void UpdateTimer()
    {
        lockonTimer.Update();
        imageTimer.Update();
    }

    /// <summary>
    /// ロックオンをするためのキーを押した時の処理
    /// </summary>
    private void CameraLockOnStart()
    {
        targetAnchor = GetTargetAnchor();
        if (targetAnchor == null) return;
        lockonTimer.TimerStart(0.2f); //ロックオンにかかる時間
        imageTimer.TimerStart(1f);
        IsLockOn = true;
        IsEndLockOn = false;
        //プレイヤーをカメラと同じ向きに向ける
        player.transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);//localEuleranglesはインスペクタと同じ数値
    }

    /// <summary>
    ///指定されたアンカーのある方向に向かってカメラを移動
    /// </summary>
    private void AnchorLockOn()
    {
        float len = (targetAnchor.transform.position - cameraObj.transform.position).magnitude;

        Vector3 lookatPosition = transform.position + (cameraObj.transform.forward * len);

        if (imageTimer.IsLimitTime) IsEndLockOn = true;
        if (IsEndLockOn)
        {
            AlignmentImage(1);
        }
        else
            AlignmentImage(imageTimer.Progress);

        Vector2 inputVec = GamePadInput.GetAxis(GamePadInput.Axis.RightStick, GamePadInput.Index.One);
        if (oldInputVec == 0)
        {
            if (inputVec.x < 0) targetAnchor = GetSideAnchor(Side.Left);

            if (inputVec.x > 0) targetAnchor = GetSideAnchor(Side.Right);
        }
        oldInputVec = inputVec.x;
        //アンカーのある方向を取得
        Vector3 vec = targetAnchor.transform.position - transform.position;
        //オブジェクトのある方向に合わせたカメラのポジション移動
        Vector3 playerPosition = player.transform.position + Vector3.up;
        transform.position = playerPosition + control.PositionForLockOnAnchor(targetAnchor);
        //カメラの注視点を移動
        //cameraObj.transform.LookAt(targetAnchor.transform);
        cameraObj.transform.LookAt(Vector3.Lerp(lookatPosition, targetAnchor.transform.position, lockonTimer.Progress));
    }

    /// <summary>
    ///照準画像の処理
    /// </summary>
    public void AlignmentImage(float timer)
    {
        AlignmentSprite.SetActive(true);
        Image img = AlignmentSprite.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, timer * timer);
        AlignmentSprite.transform.localScale = Vector3.one * 2 * ((1 - timer * timer) + 0.5f);
    }

    private void SetMaker()
    {
        GameObject obj = GetTargetAnchor();
        Image image = AlignmentSprite.GetComponent<Image>();

        //nullだったら中央に表示される
        if (obj == null)
        {
            image.rectTransform.anchoredPosition = Vector2.zero;
            return;
        }

        //アンカーがカメラのどこに表示されているか？(0～1)
        Vector3 anchorPosition = cameraObj.GetComponent<Camera>().WorldToViewportPoint(obj.transform.position);

        //canvasのrectのサイズの1/2を引く。
        float x = (anchorPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f);
        float y = (anchorPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f);

        image.rectTransform.anchoredPosition = new Vector2(x, y);
    }

    #region GetTargetAnchor
    public GameObject GetTargetAnchor()
    {
        GameObject targetAnchor = null;

        //カメラに写っているアンカーを取得
        List<GameObject> anchorList = GetViewAnchor();
        if (anchorList.Count == 0) return null;

        //見ている可能性が高いアンカーを取得
        List<GameObject> temp = GetShouldLookAnchor(anchorList);

        if (temp.Count != 0)
        {
            //5度以内のアンカーのなかで一番近いアンカーを取得
            targetAnchor = GetNearAnchor(temp);
        }
        else
        {
            //5度以内にアンカーが存在しなかったら一番角度の低いアンカーを取得
            targetAnchor = GetLowAngleAnchor(anchorList);
            //todo:一番角度の低いアンカーが取得できない場合があるっぽい
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
            if (Vector3.Distance(transform.position, n.transform.position) < distance)
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
    /// 見ている可能性の高い(5度以内)アンカーをすべて返します
    /// </summary>
    List<GameObject> GetShouldLookAnchor(List<GameObject> anchorList)
    {
        Vector2 originAnchorVec = new Vector2(transform.forward.x, cameraObj.transform.forward.z);
        anchorList = anchorList.FindAll(n =>
        {
            Vector2 vec = new Vector2(n.transform.position.x - cameraObj.transform.position.x
                                         , n.transform.position.z - cameraObj.transform.position.z);

            //左右の角度
            float tmpAngleW = Vector2.Angle(vec, originAnchorVec);
            //上下の角度
            Vector3 toAnchorVector = n.transform.position - cameraObj.transform.position;
            float tmpAngleH = control.DifferenceLatitude(toAnchorVector, cameraObj.transform.forward);
            tmpAngleH = Mathf.Abs(tmpAngleH);

            //5度以内のアンカーを検索
            return tmpAngleW < 5 && tmpAngleH < 5;
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
        Vector2 originAnchorVec = new Vector2(cameraObj.transform.forward.x, cameraObj.transform.forward.z);

        anchorList.ForEach(n =>
        {
            Vector2 vec = new Vector2(n.transform.position.x - cameraObj.transform.position.x
                             , n.transform.position.z - cameraObj.transform.position.z);

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
        if (nextAnchor == null) nextAnchor = targetAnchor;
        else lockonTimer.Reset();

        return nextAnchor;
    }
    #endregion

    /// <summary>
    /// プレイヤーの移動に合わせてカメラの位置を移動
    /// </summary>
    private void PlayerTrace()
    {
        Vector3 movement = player.transform.position - oldPlayerPosition;

        //プレイヤーが移動していなかったら終了
        if (movement.magnitude == 0) return;

        //プレイヤーについていくMOMO
        cameraTargetPosition += movement;
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
}

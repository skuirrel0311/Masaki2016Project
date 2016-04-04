using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 5;

    [SerializeField]
    private GameObject AlignmentSprite;
    private float rotationY;

    private Vector3 oldPlayerPosition;
    private GameObject player;
    private GameObject cameraObj;
    private GameObject nearAnchor;
    private Vector3 lookatPosition;
    private float timer;

    void Start()
    {
        rotationY = 0;
        player = GameObject.Find("Player");
        cameraObj = transform.FindChild("Main Camera").gameObject;
        oldPlayerPosition = player.transform.position;
    }

    void Update()
    {
        PlayerTrace();

        //ロックオンの処理押された時と押している時で処理を分ける
        if (Input.GetKeyDown(KeyCode.R))
        {
            CameraLockOnStart();
        }
        if (Input.GetKey(KeyCode.R)&&nearAnchor != null)
        {
            AnchorLockOn();
            return;
        }
        timer = Mathf.Max(timer-Time.deltaTime,0);
        float mouseX = Input.GetAxis("Horizontal2");
        AlignmentSprite.SetActive(false);
        rotationY += mouseX * rotationSpeed;

        cameraObj.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, cameraObj.transform.rotation.y, 0), cameraObj.transform.rotation, timer);
        //オブジェクトのある方向に合わせたカメラのポジション移動
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, rotationY, 0), transform.rotation, timer);
    }

    private float VectorToAngle(float a,float b)
    {
         return Mathf.Atan2(a, b) * 180.0f / Mathf.PI;
    }

    /// <summary>
    /// ロックオンをするためのキーを押した時の処理
    /// </summary>
    private void CameraLockOnStart()
    {
        nearAnchor = GetNearAnchor();
        Camera cam = cameraObj.GetComponent<Camera>();
        lookatPosition = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, cam.nearClipPlane));
        timer = 0;
    }

    /// <summary>
    ///指定されたアンカーのある方向に向かってカメラを移動
    /// </summary>
    private void AnchorLockOn()
    {
        timer = Mathf.Min(timer + Time.deltaTime, 1);
        AlignmentImage();
        //削除キーが押された場合は対象のアンカーを削除する
        if (Input.GetKeyDown(KeyCode.D))
        {
            Destroy(nearAnchor);
        }
        //アンカーのある方向を取得
        Vector3 vec = nearAnchor.transform.position - transform.position;
        //オブジェクトのある方向に合わせたカメラのポジション移動
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, VectorToAngle(vec.x, vec.z), 0), timer);
        //カメラの注視点を移動
        cameraObj.transform.LookAt(Vector3.Lerp(lookatPosition, nearAnchor.transform.position, timer));
    }

    private void AlignmentImage()
    {
        AlignmentSprite.SetActive(true);
        Image img = AlignmentSprite.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, timer * timer);
        AlignmentSprite.transform.localScale = Vector3.one * 2 * ((1 - timer * timer )+ 0.5f);
    }

    //todo:CreateFlowとメソッドがかぶっているのでライブラリを作成する
    GameObject GetNearAnchor()
    {
        GameObject gameObject=null;
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

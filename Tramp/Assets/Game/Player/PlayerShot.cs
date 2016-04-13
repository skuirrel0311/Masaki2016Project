using UnityEngine;
using System.Collections;
using GamepadInput;

public class PlayerShot : MonoBehaviour
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

    void Start()
    {
        playerNum = GetComponent<PlayerControl>().playerNum;
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("Camera" + playerNum);
        cam = cameraObj.GetComponentInChildren<Camera>();
        StartCoroutine("LongButtonDown");
    }

    void Shot()
    {
        //カメラの中心座標からレイを飛ばす
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));
        RaycastHit hit;
        Vector3 targetPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hit, 100))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = ray.origin + (ray.direction * 100);
        }
        
        //先にプレイヤーをカメラと同じ方向に向ける
        transform.rotation = cameraObj.transform.rotation;

        shotPosition.LookAt(targetPosition);
        Instantiate(Ammo, shotPosition.position, shotPosition.rotation);


    }

    IEnumerator LongButtonDown()
    {
        while (true)
        {
            //アイテムを持っているときはショットは打てない
            if(GamePad.GetTrigger(GamePad.Trigger.RightTrigger,(GamePad.Index)playerNum,true)>0 && !playerState.IsPossessionOfItem)
                Shot();
            yield return new WaitForSeconds(shotDistance);
        }
    }
}

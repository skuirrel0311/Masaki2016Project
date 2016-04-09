using UnityEngine;
using System.Collections;
using GamepadInput;

public class PlayerShot : MonoBehaviour
{

    [SerializeField]
    GameObject Ammo;

    [SerializeField]
    GameObject cameraObj;
    
    private int playerNum;

    /// <summary>
    /// 弾の発射される位置
    /// </summary>
    [SerializeField]
    private Transform shotPosition;

    [SerializeField]
    float shotDistance;

    void Start()
    {
        playerNum = GetComponent<PlayerControl>().playerNum;
        StartCoroutine("LongButtonDown");
    }

    void Shot()
    {
        Camera cam = cameraObj.GetComponentInChildren<Camera>();
        Ray ray = cam.ViewportPointToRay(new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0));
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
        
        transform.rotation = cameraObj.transform.rotation;

        shotPosition.LookAt(targetPosition);
        Instantiate(Ammo, shotPosition.position, shotPosition.rotation);


    }

    IEnumerator LongButtonDown()
    {
        while (true)
        {
            if(GamePad.GetButton(GamePad.Button.B, (GamePad.Index)playerNum))
                Shot();
            yield return new WaitForSeconds(shotDistance);
        }
    }
}

using UnityEngine;
using System.Collections;
using GamepadInput;

public class PlayerShot : MonoBehaviour
{

    [SerializeField]
    GameObject Ammo;

    [SerializeField]
    GameObject mainCamera;
    CameraControl cameraControl;

    [SerializeField]
    int playerNo;

    [SerializeField]
    float shotDistance;

    void Start()
    {
        cameraControl = mainCamera.GetComponent<CameraControl>();
        StartCoroutine("LongButtonDown");
    }

    void Update()
    {
    }

    void Shot()
    {
        cameraControl.shotPosition.LookAt(cameraControl.targetPosition);
        Instantiate(Ammo, cameraControl.shotPosition.position, cameraControl.shotPosition.rotation);

        transform.rotation = mainCamera.transform.rotation;
    }

    IEnumerator LongButtonDown()
    {
        while (true)
        {
            if(GamePad.GetButton(GamePad.Button.B, (GamePad.Index)playerNo))
                Shot();
            yield return new WaitForSeconds(shotDistance);
        }
    }
}

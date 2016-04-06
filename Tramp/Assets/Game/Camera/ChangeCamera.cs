using UnityEngine;
using System.Collections;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField]
    Camera thirdPersonCamera;
    [SerializeField]
    Camera firstPersonCamera;
    [SerializeField]
    GameObject nozzle;

    CameraControl cameraControl;

    void Start()
    {
        firstPersonCamera.enabled = false;
        cameraControl = GetComponent<CameraControl>();
        nozzle.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire5"))
        {
            thirdPersonCamera.enabled = false;
            firstPersonCamera.enabled = true;
            cameraControl.enabled = false;
            nozzle.SetActive(true);
        }

        if (Input.GetButtonUp("Fire5"))
        {
            thirdPersonCamera.enabled = true;
            firstPersonCamera.enabled = false;
            cameraControl.enabled = true;
            nozzle.SetActive(false);
        }
    }
}

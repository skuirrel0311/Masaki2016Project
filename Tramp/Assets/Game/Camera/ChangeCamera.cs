using UnityEngine;
using System.Collections;

public class ChangeCamera : MonoBehaviour
{
    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;

    void Start()
    {
        firstPersonCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire5"))
        {
            thirdPersonCamera.enabled = false;
            firstPersonCamera.enabled = true;
        }

        if (Input.GetButtonUp("Fire5"))
        {
            thirdPersonCamera.enabled = true;
            firstPersonCamera.enabled = false;
        }
    }
}

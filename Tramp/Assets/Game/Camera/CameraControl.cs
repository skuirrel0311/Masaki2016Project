using UnityEngine;
using System.Collections;
using GamepadInput;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 5;
    private float rotationY;

    private Vector3 oldPlayerPosition;
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private int playerNo;
    
    public Transform targetPosition;
    
    public Transform shotPosition;

    void Start()
    {
        rotationY = 0;
        oldPlayerPosition = player.transform.position;
    }

    void Update()
    {
        float mouseX = GamePad.GetAxis(GamePad.Axis.RightStick,(GamePad.Index)playerNo).x;

        rotationY += mouseX * rotationSpeed;

        transform.rotation = Quaternion.Euler(0, rotationY, 0);

        Vector3 movement = player.transform.position - oldPlayerPosition;

        //プレイヤーが移動していなかったら終了
        if (movement.magnitude == 0) return;

        //プレイヤーについていく
        transform.position += movement;

        oldPlayerPosition = player.transform.position;
    }
}

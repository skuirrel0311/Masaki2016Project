using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{
    public float rotationSpeed = 5;
    private float rotationY;

    private Vector3 oldPlayerPosition;
    private GameObject player;

    void Start()
    {
        rotationY = 0;
        player = GameObject.Find("Player");
        oldPlayerPosition = player.transform.position;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");

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

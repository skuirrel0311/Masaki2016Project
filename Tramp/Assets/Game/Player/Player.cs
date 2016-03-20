using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    public float rotationSpeed = 360;

    private Animator animator;
    private GameObject mainCamera;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = GameObject.Find("Camera");
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //入力の角度をカメラの角度に曲げる
        direction = mainCamera.transform.rotation * direction;

        animator.SetFloat("Speed", direction.magnitude);

        //移動していなかったら終了
        if (direction == Vector3.zero) return;

        //弧を描くように移動
        Vector3 forward = Vector3.Slerp(
            transform.forward,  //正面から
            direction,          //入力の角度まで
            rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction)
            );
        //向きを変える
        transform.LookAt(transform.position + forward);

        transform.Translate(direction * moveSpeed * Time.deltaTime,Space.World);
    }
}

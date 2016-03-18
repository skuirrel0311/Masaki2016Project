using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    public float rotationSpeed = 360;
    public GameObject instanceAnchor;

    private Animator animator;
    private GameObject camera;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        camera = GameObject.Find("Camera");
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.X))
        {
            CreateAnchor();
        }
    }

    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //入力の角度をカメラの角度に曲げる
        direction = camera.transform.rotation * direction;

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

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void CreateAnchor()
    {
        Instantiate(instanceAnchor, transform.position, transform.rotation);
    }
}

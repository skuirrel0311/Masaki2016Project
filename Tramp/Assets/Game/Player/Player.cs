using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    public float rotationSpeed = 360;

    private Animator animator;
    private GameObject mainCamera;

    /*ジャンプ系クラスに分けるべき？*/
    public Vector3 jumpVec = new Vector3(0, 0.3f, 0);
    //ジャンプキーを長押しした際の上限
    public float jumpLimitPositionY = 5;
    //カメラは上下には動かない(予定)
    public bool IsJump { get; private set; }
    //ジャンプして落ちてる
    private bool IsDropDown;
    //ジャンプキーが押された時の座標
    private Vector3 atJumpPosition;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = GameObject.Find("Camera");
        atJumpPosition = Vector3.zero;
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //入力の角度をカメラの角度に曲げる
        direction = mainCamera.transform.rotation * direction;

        //アニメーターにパラメータを送る
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

        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    void Jump()
    {
        //アニメーターにパラメータを送る
        animator.SetBool("Jump", IsJump);

        //プレイヤーがジャンプをしようとしたとき
        if (Input.GetButtonDown("Jump") && IsJump == false && IsDropDown == false)
        {
            //ジャンプ時の地点を保持
            atJumpPosition = transform.position;
            IsJump = true;
        }

        //ジャンプキー長押し中
        if (Input.GetButton("Jump") && IsDropDown == false)
        {
            //最高地点に達した
            if (transform.position.y >= atJumpPosition.y + jumpLimitPositionY)
            {
                IsDropDown = true;
            }
            transform.position += jumpVec;
        }

        //ジャンプキーを離した
        if (Input.GetButtonUp("Jump"))
        {
            IsDropDown = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Landed(collision);
    }

    //着地した
    void Landed(Collision collision)
    {
        //ジャンプして落ちていなかったら
        if (IsDropDown == false) return;

        if (collision.gameObject.tag == "Plane" || collision.gameObject.tag == "Anchor")
        {
            IsJump = false;
            IsDropDown = false;

            //着地時ジャンプキーを押したままなら
            if (Input.GetKey(KeyCode.Space))
            {
                //ジャンプ時の地点を保持
                atJumpPosition = transform.position;
                IsJump = true;
                animator.SetBool("Jump", false);
            }
        }
    }
}

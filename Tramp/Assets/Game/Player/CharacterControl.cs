using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 5;
    public float rotationSpeed = 360;

    private Animator animator;
    private GameObject ThirdPersonCamera;

    [SerializeField]
    private Vector3 jumpVec = new Vector3(0, 0.3f, 0);

    /// <summary>
    /// ジャンプキーを長押しした際の高さ上限
    /// </summary>
    public float jumpLimitPositionY = 5;

    /// <summary>
    /// ジャンプ中か？
    /// </summary>
    private bool IsJumping;

    /// <summary>
    /// 落下している
    /// </summary>
    private bool IsFalling;

    /// <summary>
    /// 地面にいるか？(重力の影響を受けない予定)
    /// </summary>
    private bool IsOnGround;

    /// <summary>
    /// ジャンプキーが押された時の座標
    /// </summary>
    private Vector3 positionAtJump;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ThirdPersonCamera = GameObject.Find("Camera");
        positionAtJump = Vector3.zero;
        IsOnGround = true;
    }
    void Update()
    {
        //入力で移動ベクトルを取る
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Move(direction);
        Jump();

        //アニメーターにパラメータを送る
        animator.SetFloat("Speed", direction.magnitude);
        animator.SetBool("Jump", IsJumping);

    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="movement">移動量</param>
    void Move(Vector3 movement)
    {
        //入力の角度をカメラの角度に曲げる
        movement = ThirdPersonCamera.transform.rotation * movement;

        //移動していなかったら終了
        if (movement == Vector3.zero) return;

        //弧を描くように移動
        Vector3 forward = Vector3.Slerp(
            transform.forward,  //正面から
            movement,          //移動の方向まで
            rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, movement)
            );
        //向きを変える
        transform.LookAt(transform.position + forward);

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }

    void Jump()
    {
        //プレイヤーがジャンプをしようとしたとき
        if (Input.GetButtonDown("Jump") && IsOnGround)
        {
            //ジャンプ時の地点を保持
            positionAtJump = transform.position;
            IsJumping = true;
            IsOnGround = false;
        }

        //ジャンプキー長押し中
        if (Input.GetButton("Jump") && IsFalling == false && IsJumping == true)
        {
            //最高地点に達した
            if (transform.position.y >= positionAtJump.y + jumpLimitPositionY)
            {
                IsFalling = true;
            }
            transform.position += jumpVec;
        }

        //ジャンプキーを離した
        if (Input.GetButtonUp("Jump") && IsJumping == true)
        {
            IsFalling = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Landed(collision);
    }

    /// <summary>
    /// 着地した
    /// </summary>
    void Landed(Collision collision)
    {
        //ジャンプして落ちていなかったら
        if (IsFalling == false || IsOnGround) return;
        if (collision.gameObject.tag != "Plane" && collision.gameObject.tag != "Anchor") return;

        IsJumping = false;
        IsFalling = false;
        IsOnGround = true;
    }
}

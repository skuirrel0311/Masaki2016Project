using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GamepadInput;

public class PlayerControl : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 5;         //移動速度
    private float rotationSpeed = 360;   //振り向きの際数値が大きいとゆっくりになる

    private Animator animator;
    [SerializeField]
    private GameObject mainCamera;//プレイヤーが使用するカメラ

    [SerializeField]
    private Vector3 jumpVec = new Vector3(0, 0.3f, 0); //ジャンプ力

    [SerializeField]
    private float jumpLimitPositionY = 5; //ジャンプキーを長押しした際の上限

    /// <summary>
    /// 地上にいる
    /// </summary>
    public bool IsOnGround { get; private set; }

    /// <summary>
    /// ジャンプ中
    /// </summary>
    public bool IsJumping { get; private set; }

    /// <summary>
    /// 落下中
    /// </summary>
    public bool Isfalling { get; private set; }

    /// <summary>
    /// ジャンプキーが押された時の座標
    /// </summary>
    private Vector3 atJumpPosition;
    
    public int playerNum;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        atJumpPosition = Vector3.zero;
        IsOnGround = true;
        if (isLocalPlayer)
        {
            GameObject.Find("Camera1").GetComponent<CameraControl>().SetPlayer(gameObject);
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    void Update()
    {
        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, (GamePad.Index)playerNum);
        Vector3 direction = new Vector3(leftStick.x,0,leftStick.y);
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
        //カメラの角度のx､zは見ない
        Quaternion cameraRotation = mainCamera.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        //入力の角度をカメラの角度に曲げる
        movement = cameraRotation * movement;

        //移動していなかったら終了
        if (movement == Vector3.zero) return;

        //弧を描くように移動
        Vector3 forward = Vector3.Slerp(
            transform.forward,  //正面から
            movement,          //入力の角度まで
            rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, movement)
            );
        //向きを変える
        transform.LookAt(transform.position + forward);

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }

    void Jump()
    {
        //プレイヤーがジャンプをしようとしたとき
        if (GamePad.GetButtonDown(GamePad.Button.A, (GamePad.Index)playerNum) && IsOnGround)
        {
            //ジャンプ時の地点を保持
            atJumpPosition = transform.position;
            IsJumping = true;
            IsOnGround = false;
        }

        //ジャンプキー長押し中
        if (GamePad.GetButton(GamePad.Button.A, (GamePad.Index)playerNum) && Isfalling == false && IsJumping == true)
        {
            //最高地点に達した
            if (transform.position.y >= atJumpPosition.y + jumpLimitPositionY)
            {
                Isfalling = true;
            }
            transform.position += jumpVec;
        }

        //ジャンプキーを離した
        if (GamePad.GetButtonUp(GamePad.Button.A, (GamePad.Index)playerNum) && IsJumping == true)
        {
            Isfalling = true;
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
        if (Isfalling == false) return;
        if (collision.gameObject.tag != "Plane" && collision.gameObject.tag != "Anchor") return;

        IsJumping = false;
        Isfalling = false;
        IsOnGround = true;
    }
}

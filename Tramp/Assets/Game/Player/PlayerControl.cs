using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GamepadInput;

public class PlayerControl : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 1000;         //移動速度
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
    [SerializeField]
    private bool IsOnGround;

    /// <summary>
    /// ジャンプ中
    /// </summary>
    [SerializeField]
    private bool IsJumping;

    /// <summary>
    /// 落下中
    /// </summary>
    [SerializeField]
    private bool Isfalling;

    /// <summary>
    /// ジャンプキーが押された時の座標
    /// </summary>
    private Vector3 atJumpPosition;
    
    public int playerNum;

    private Rigidbody body;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        body = GetComponent<Rigidbody>();
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
        body.isKinematic = false;
        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, (GamePad.Index)playerNum);
        Vector3 direction = new Vector3(leftStick.x,0,leftStick.y);
        Move(direction);
        Jump();

        //アニメーターにパラメータを送る
        //animator.SetFloat("Speed", direction.magnitude);
        //animator.SetBool("Jump", IsJumping);
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="movement">移動量</param>
    void Move(Vector3 movement)
    {
        //移動していなかったら終了
        if (movement == Vector3.zero) return;

        body.velocity = new Vector3(0,body.velocity.y,0);
        //カメラの角度のx､zは見ない
        Quaternion cameraRotation = mainCamera.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;
        //入力の角度をカメラの角度に曲げる
        movement = cameraRotation * movement;

        //弧を描くように移動
        Vector3 forward = Vector3.Slerp(
            transform.forward,  //正面から
            movement,          //入力の角度まで
            rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, movement)
            );
        //向きを変える
        transform.LookAt(transform.position + forward);
        body.AddForce(movement * moveSpeed,ForceMode.VelocityChange);
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
            //body.isKinematic = true;
            body.AddForce(jumpVec*100,ForceMode.Impulse);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player Control hit");
        //地面にいたらダメ
        if (IsOnGround) return;

        //Areaはトリガーなのでヒットしないが
        //どうやら親のタグを取得しているみたい
        if (collision.gameObject.tag != "Plane" && collision.gameObject.tag != "Anchor" && collision.gameObject.tag != "Area") return;
        Landed();
    }

    //着地した
    void Landed()
    {
        IsJumping = false;
        Isfalling = false;
        IsOnGround = true;
        body.isKinematic = false;
    }
}

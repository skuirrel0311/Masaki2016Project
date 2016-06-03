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
    public bool IsOnGround;
    //落下してから着地した
    public bool IsFallAfter;

    /// <summary>
    /// 流れに乗っている
    /// </summary>
    public bool IsFlowing;

    [SerializeField]
    private float EndArea = 59;

    /// <summary>
    /// ジャンプ中
    /// </summary>
    [SerializeField]
    public bool IsJumping;

    /// <summary>
    /// 落下中
    /// </summary>
    [SerializeField]
    public bool IsFalling;

    /// <summary>
    /// ジャンプキーが押された時の座標
    /// </summary>
    private Vector3 atJumpPosition;

    public int playerNum;

    private Rigidbody body;
    bool isRun = false;

    Timer onGroundTimer = new Timer();
    public Timer OnGroundTimer { get { return onGroundTimer; } }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        body = GetComponent<Rigidbody>();
        atJumpPosition = transform.position;
        IsOnGround = false;
        IsFalling = true;
        isRun = false;

        if (isLocalPlayer)
        {
            SetSratPosition();
            GameObject.Find("Camera1").GetComponent<CameraControl>().SetPlayer(gameObject);
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    void OnDestroy()
    {
        Debug.Log("Player Destory");
    }

    public void SetSratPosition()
    {
        //スタート座標の設定
        if (isServer)
        {
            GameObject go = GameObject.Find("HostStart");
            transform.position = go.transform.position;
            transform.rotation = go.transform.rotation;
        }
        else
        {
            GameObject go = GameObject.Find("ClientStart");
            transform.position = go.transform.position;
            transform.rotation = go.transform.rotation;
        }
    }

    void Update()
    {
        onGroundTimer.Update();
        body.isKinematic = false;
        Vector2 leftStick = GamePadInput.GetAxis(GamePadInput.Axis.LeftStick, (GamePadInput.Index)playerNum);
        Vector3 direction = new Vector3(leftStick.x, 0, leftStick.y);
        Move(direction);
        Jump();

        //ジャンプしてジャンプ開始地点よりも下に落ちた
        if (IsJumping && atJumpPosition.y > transform.position.y)
        {
            IsFalling = true;
        }

        //アニメーターにパラメータを送る
        if (!ChackCurrentAnimatorName(animator, "wait") && !Move(direction))
        {
            isRun = false;
            animator.SetBool("IsRun", isRun);
        }

        Vector3 c = new Vector3(transform.position.x, 0, transform.position.z);
        if (c.magnitude > EndArea)
        {
            c.Normalize();
            transform.position = new Vector3(c.x * EndArea, transform.position.y, c.z * EndArea);
        }
    }

    static public bool ChackCurrentAnimatorName(Animator animator, string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="movement">移動量</param>
    bool Move(Vector3 movement)
    {
        //ポーズ中だったら終了
        if (MainGameManager.IsPause) return false;

        // body.velocity = new Vector3(0,body.velocity.y,0);
        //カメラの角度のx､zは見ない
        Quaternion cameraRotation = mainCamera.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        Vector3 mov = movement;
        //入力の角度をカメラの角度に曲げる
        movement = cameraRotation * movement;

        //移動していなかったら終了
        if (movement == Vector3.zero) return false;


        //アニメーションの再生
        if (!ChackCurrentAnimatorName(animator, "Take 001"))
        {
            isRun = true;
            animator.SetBool("IsRun", isRun);
        }


        //向きを変える
        if (mov.z >= 0)
        {
            //弧を描くように移動
            Vector3 forward = Vector3.Slerp(
                transform.forward,  //正面から
                movement,          //入力の角度まで
                rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, movement)
                );
            transform.LookAt(transform.position + forward);
        }
        else
        {
            //弧を描くように移動
            Vector3 forward = Vector3.Slerp(
                transform.forward,  //正面から
                -movement,          //入力の角度まで
                rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, -movement)
                );
            transform.LookAt(transform.position + forward);
        }

        //body.AddForce(movement * moveSpeed,ForceMode.VelocityChange);
        transform.Translate(movement * Time.deltaTime * moveSpeed, Space.World);
        return true;
    }

    void Jump()
    {
        //プレイヤーがジャンプをしようとしたとき
        if (GamePadInput.GetButtonDown(GamePadInput.Button.A, (GamePadInput.Index)playerNum) && IsOnGround && !MainGameManager.IsPause)
        {
            //ジャンプ時の地点を保持
            atJumpPosition = transform.position;
            IsJumping = true;
            IsOnGround = false;
            animator.CrossFadeInFixedTime("jump", 0.5f);
            //body.isKinematic = true;
            body.AddForce(jumpVec * 100, ForceMode.Impulse);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        //地面にいたらダメ
        if (IsOnGround) return;

        if (collision.gameObject.name == "FixAnchor") AnchorHit();

        //Areaはトリガーなのでヒットしないが
        //どうやら親のタグを取得しているみたい
        if (collision.gameObject.tag != "Plane" && collision.gameObject.tag != "Area" && collision.gameObject.tag != "Scaffold") return;
        Landed();
    }

    public void AnchorHit()
    {
        IsFlowing = false;
        IsFalling = true;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag != "Plane" && col.gameObject.tag != "Area" && col.gameObject.tag != "Scaffold") return;

        //床から離れたら落ちているかジャンプしているか

        //ジャンプもしてない、流れてもいない、なのに地面から離れてたら
        if (!IsJumping && !IsOnGround && !IsFlowing) IsFalling = true;
        animator.CrossFadeInFixedTime("jump", 0.5f);
        IsOnGround = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "FixAnchor") AnchorHit();

        //着地した
        if (col.tag == "Box") Landed();
    }

    void OnTriggerExit(Collider col)
    {
        //地面から離れた
        if (col.tag != "Box" && col.tag != "Scaffold") return;

        //ジャンプもしてない、流れてもいない、なのに地面から離れてたら
        if (!IsJumping && !IsOnGround && !IsFlowing) IsFalling = true;
        animator.CrossFadeInFixedTime("jump", 0.5f);
        IsOnGround = false;
    }
    
    //着地した
    void Landed()
    {
        if (IsFalling) IsFallAfter = true;
        IsJumping = false;
        IsFalling = false;
        IsOnGround = true;
        IsFlowing = false;
        body.isKinematic = false;
        animator.CrossFadeInFixedTime("jump_landing", 0.1f);
        onGroundTimer.TimerStart(0.4f);
        atJumpPosition = transform.position;
    }
}

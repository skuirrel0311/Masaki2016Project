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
    private GameObject cameraObj;//プレイヤーが使用するカメラ
    private CameraControl cameraControl;

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
    public Vector3 atJumpPosition;

    public int playerNum;

    private Rigidbody body;
    bool isRun = false;

    Timer onGroundTimer = new Timer();
    public Timer OnGroundTimer { get { return onGroundTimer; } }
    Timer landedTimer = new Timer();
    public Timer LandedTimer { get { return landedTimer; } }
    Timer fallTimer = new Timer();

    Vector3 movement = Vector3.zero;

    //最後にあたっていた流れ
    public bool hitFix;

    [SerializeField]
    GameObject RideEffect;

    bool  landingEnd;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        body = GetComponent<Rigidbody>();
        atJumpPosition = transform.position;
        IsOnGround = false;
        IsFalling = true;
        isRun = false;
        hitFix = false;
        landingEnd = false;
        fallTimer.TimerStart(1);
        if (isLocalPlayer)
        {
            SetSratPosition();
            cameraControl = GameObject.Find("Camera1").GetComponent<CameraControl>();
            cameraControl.SetPlayer(gameObject);
            cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        }

        RideEffect.SetActive(false);
    }

    void OnDestroy()
    {
        GameObject nm = GameObject.FindGameObjectWithTag("NetworkManager");
        if (nm == null) return;
        if (nm.GetComponent<MyNetworkManager>().autoCreatePlayer && isLocalPlayer)
        {
            short s = (short)(playerControllerId + 1);
            ClientScene.AddPlayer(s);
        }

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

    void FixedUpdate()
    {
        Vector2 leftStick = GamePadInput.GetAxis(GamePadInput.Axis.LeftStick, (GamePadInput.Index)playerNum);
        movement = new Vector3(leftStick.x, 0, leftStick.y);
        //アニメーターにパラメータを送る
        bool ismove = Move();

        if(ismove&& ChackCurrentAnimatorName(animator, "jump_landing"))
        {
            if (landingEnd) return;
            landingEnd = true;
            animator.CrossFadeInFixedTime("Take 001", 0.2f);
            isRun = true;
        }
        else
        {
            landingEnd = false;
        }
        //街のモーションでなく、動いていなければ待ちのモーションにする
        if (!ChackCurrentAnimatorName(animator, "wait") && !ismove)
        {
            isRun = false;
            animator.SetBool("IsRun", isRun);
        }
    }

    void Update()
    {
        UpdateTimer();


        Jump();

        //ジャンプしてジャンプ開始地点よりも下に落ちた
        if (IsJumping && atJumpPosition.y > transform.position.y)
        {
            if (!IsFalling) cameraControl.SetNowLatitude();
            cameraControl.IsEndFallingCamera = false;
            IsFalling = true;
        }



        Vector3 c = new Vector3(transform.position.x, 0, transform.position.z);
        if (c.magnitude > EndArea)
        {
            c.Normalize();
            transform.position = new Vector3(c.x * EndArea, transform.position.y, c.z * EndArea);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        if (GetComponent<Rigidbody>().useGravity == false)
        {
            RideEffect.SetActive(true);
            RideEffect.transform.LookAt(RideEffect.transform.position - GetComponent<Rigidbody>().velocity.normalized);
        }
        else
        {
            RideEffect.SetActive(false);
        }
    }

    void UpdateTimer()
    {
        onGroundTimer.Update();
        landedTimer.Update();
        fallTimer.Update();
    }

    static public bool ChackCurrentAnimatorName(Animator animator, string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="movement">移動量</param>
    bool Move()
    {
        //ポーズ中だったら終了
        if (MainGameManager.IsPause) return false;

        // body.velocity = new Vector3(0,body.velocity.y,0);
        //カメラの角度のx､zは見ない
        Quaternion cameraRotation = cameraObj.transform.rotation;
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
                transform.forward,  //正面からM
                -movement,          //入力の角度まで
                rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, -movement)
                );
            transform.LookAt(transform.position + forward);
        }

        //body.AddForce(movement * moveSpeed,ForceMode.VelocityChange);
        movement = movement * Time.deltaTime * moveSpeed;
        transform.Translate(movement, Space.World);
        return true;
    }

    void Jump()
    {
        //プレイヤーがジャンプをしようとしたとき
        if (GamePadInput.GetButtonDown(GamePadInput.Button.A, (GamePadInput.Index)playerNum) && !IsJumping && !MainGameManager.IsPause)
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

    IEnumerator SleepHItFix()
    {
        yield return new WaitForSeconds(1);
        hitFix = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        //地面にいたらダメ
        if (IsOnGround) return;

        if (collision.gameObject.name == "FixAnchor") AnchorHit();

        //どうやら親のタグを取得しているみたい
        if (collision.gameObject.tag != "Plane" && collision.gameObject.tag != "Scaffold" && collision.gameObject.tag != "Area") return;
        Landed();
    }

    public void AnchorHit()
    {
        IsFlowing = false;
        IsFalling = true;
        cameraControl.SetNowLatitude();
        cameraControl.IsEndFallingCamera = false;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag != "Plane") return;

        //ジャンプもしてない、流れてもいない、なのに地面から離れてたら
        if (!IsJumping && !IsFlowing)
        {
            Fall();
        }
        animator.CrossFadeInFixedTime("jump", 0.5f);
        IsOnGround = false;
    }

    public void Fall()
    {
        animator.CrossFadeInFixedTime("jump", 0.5f);
        IsOnGround = false;
        if (!IsFalling) cameraControl.SetNowLatitude();
        cameraControl.IsEndFallingCamera = false;
        IsFalling = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "FixAnchor" || col.name == "AreaAnchor")
        {
            hitFix = true;
            if (col.name == "AreaAnchor")
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().useGravity = true;
            }
        }
        if (col.gameObject.name == "FixAnchor") AnchorHit();

        //着地した
        if (col.tag == "Box") Landed();
    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log("player trigger exit = " + col.tag);

        if (col.name == "FixAnchor" || col.name == "AreaAnchor")
        {
            StartCoroutine("SleepHItFix");
        }
        //地面から離れた
        if (col.tag != "Box" && col.tag != "Scaffold") return;

        //ジャンプもしてない、流れてもいない、なのに地面から離れてたら
        if (!IsJumping && !IsFlowing)
        {
            Fall();
        }
        animator.CrossFadeInFixedTime("jump", 0.5f);
        IsOnGround = false;
    }

    //着地した
    void Landed()
    {
        if (IsFalling)
        {
            IsFallAfter = true;
            landedTimer.TimerStart(0.5f);
        }
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

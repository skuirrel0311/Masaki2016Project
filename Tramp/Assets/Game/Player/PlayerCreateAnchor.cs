using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GamepadInput;

public class PlayerCreateAnchor : NetworkBehaviour
{

    [SerializeField]
    GameObject InstanceAnchor;

    [SerializeField]
    float UncreateDistance = 3;

    [SerializeField]
    GameObject FlowEffect;

    GameObject camera;

    private int playerNum;
    PlayerState playerState;
    AppealAreaState appealArea;

    GameObject cameraObj;
    GameObject targetAnchor=null;
    
    Vector3 flowVector;
    Vector3 targetPosition;
    Vector3 CreatePosition;
    
    float collsionRadius = 1;

    /// <summary>
    /// アピールエリアに繋がっている流れか？
    /// </summary>
    public bool IsFromArea = false;

    // Use this for initialization
    void Start()
    {
        playerNum = GetComponentInParent<PlayerControl>().playerNum;
        camera = GameObject.Find("Camera1");
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("ThirdPersonCamera");
        appealArea = GameObject.Find("AppealArea").GetComponent<AppealAreaState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePadInput.GetTrigger(GamePadInput.Trigger.LeftTrigger,GamePadInput.Index.One)==1.0f&&camera.GetComponent<CameraControl>().IsLockOn)
        {
            if (MainGameManager.IsPause) return;
            if (!playerState.IsOnAppealArea && !CheckNearAnchor())
                return;

            camera.GetComponent<CameraControl>().IsLockOn = false;
            Debug.Log("start");
            
            //アンカーを置く
            CreateAnchor();

            //アピールエリアにいるが所有権を持っていなかったらリターン
            if (playerState.IsOnAppealArea && !playerState.IsAreaOwner)
                return;
            
            //流れを繋ぐ先を取得する
            GetTargetAnchor();

            //アピールエリアに繋ぐ流れは壁をすり抜けない
            if (playerState.IsAreaOwner && !IsPossibleCreateFlow())
                return;

            //流れを生成する
            CmdCreateFlowObject(targetPosition, CreatePosition, flowVector,IsFromArea);
        }
    }

    void GetTargetAnchor()
    {
        CameraControl camControl = camera.GetComponent<CameraControl>();
        targetAnchor = camControl.targetAnchor;
        float distance = 1000000;
        if (targetAnchor != null)
        {
            targetPosition = targetAnchor.transform.position;
            flowVector = targetPosition - CreatePosition;
            distance = flowVector.magnitude;
            return;
        }

        targetAnchor = camControl.GetTargetAnchor();
        if (targetAnchor != null)
        {
            targetPosition = targetAnchor.transform.position;
            flowVector = targetPosition - CreatePosition;
            distance = flowVector.magnitude;
            return;
        }

        //一番近いアンカーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");
        if (objects.Length <= 0) return;
        foreach (GameObject obj in objects)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) < distance)
            {
                targetAnchor = obj;
                targetPosition = obj.transform.position;
                flowVector = targetPosition - CreatePosition;
                distance = flowVector.magnitude;
            }
        }
    }

    //他のアンカーが近すぎないかチェック
    bool CheckNearAnchor()
    {
        //一番近いアンカーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");
        if (objects.Length <= 0) return true;

        float MinimumDistance = 1000000;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < MinimumDistance)
            {
                MinimumDistance = distance;
            }
        }

        if (MinimumDistance < UncreateDistance)
        {
            return false;
        }
        return true;

    }

    [ClientCallback]
    void CreateAnchor()
    {
        //カメラの向いている方向にプレイヤーを向ける
        float rotationY = cameraObj.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);

        //アピールエリアに乗っていた場合の処理
        if (playerState.IsOnAppealArea)
        {
            //まだ流れに繋がっていなかったら所有権を得る
            if (!appealArea.IsFlowing)
            {
                appealArea.SetOwner(gameObject);
                playerState.IsAreaOwner = true;
            }
            //所有者だったら流れを生成することが出来る
            if (playerState.IsAreaOwner)
            {
                IsFromArea = true;
                CreatePosition = appealArea.gameObject.transform.position;
            }
        }
        else
        {
            IsFromArea = false;
            CreatePosition = transform.position + transform.forward * 2 + Vector3.up;
            //アンカーを置く
            Cmd_rezobjectonserver(CreatePosition);
        }
        Debug.Log("clientCallend");
    }

    [Command]
    public void Cmd_rezobjectonserver(Vector3 createPosition)
    {
        Debug.Log("end1");
        GameObject obj;
        obj = Instantiate(InstanceAnchor,createPosition, transform.rotation) as GameObject;
        obj.GetComponent<CreateFlow>().SetCreatePlayerIndex(1);
        NetworkServer.Spawn(obj);
        Debug.Log("end2");
    }

    [Command]
    void CmdCreateFlowObject(Vector3 tpos,Vector3 thisPositon, Vector3 flowvec,bool isfrom)
    {
        if (!isServer) return;
        //流れのコリジョン用オブジェクト
        GameObject boxCol = Instantiate(FlowEffect);
        boxCol.transform.localScale = new Vector3(2, flowvec.magnitude * 0.5f, 2);

        //CapsuleColliderをアタッチする
        CapsuleCollider capcol = boxCol.GetComponent<CapsuleCollider>();
        capcol.height = flowvec.magnitude / (flowvec.magnitude * 0.5f);
        capcol.radius = collsionRadius / 2;
        capcol.isTrigger = true;

        //FlowScriptをアタッチする
        Flow flow = boxCol.GetComponent<Flow>();
        flow.FlowVector = flowvec;
        flow.TargetPosition = tpos;
        flow.IsFromArea = isfrom;

        //流れのベクトルに合わせて回転させる
        float dist = Vector3.Distance(tpos, thisPositon);
        float leap = ((1.5f + dist) / dist) * 0.5f;//少し出す位置をずらす
        boxCol.transform.position = Vector3.Lerp(tpos, thisPositon, leap);
        boxCol.transform.rotation = Quaternion.FromToRotation(Vector3.up,flowvec.normalized);
        NetworkServer.Spawn(boxCol);
    }

    bool IsPossibleCreateFlow()
    {
        //アピールエリアに繋ぐ流れは壁をすり抜けない
        Ray ray = new Ray(appealArea.transform.position + Vector3.up, flowVector);
        float radius = 1;
        RaycastHit hit;
        if(Physics.SphereCast(ray, radius, out hit))
        {
            //あたったのが床だったらダメ
            if(hit.transform.gameObject.tag == "Plane")
                return false;
        }

        return true;
    }

}

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

    GameObject cameraObj;
    GameObject targetAnchor=null;
    
    Vector3 flowVector;
    Vector3 targetPosition;
    Vector3 CreatePosition;
    
    float collsionRadius = 1;

    /// <summary>
    /// アピールエリアから繋がっている流れか？
    /// </summary>
    public bool IsFromArea = false;

    /// <summary>
    /// アピールエリアへ繋ぐ流れか？
    /// </summary>
    public bool IsToArea = false;

    // Use this for initialization
    void Start()
    {
        playerNum = GetComponentInParent<PlayerControl>().playerNum;
        camera = GameObject.Find("Camera1");
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("ThirdPersonCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePadInput.GetTrigger(GamePadInput.Trigger.LeftTrigger, GamePadInput.Index.One) == 1.0f)
        {
            if (MainGameManager.IsPause) return;
            if ( !CheckNearAnchor())
                return;

            camera.GetComponent<CameraControl>().IsLockOn = false;
            Debug.Log("start");
            
            //アンカーを置く
            CreateAnchor();

            GetTargetAnchor();
            //流れを生成する
            CmdCreateFlowObject(targetPosition, CreatePosition, flowVector,isServer);
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


            CreatePosition = transform.position + transform.forward * 2 + Vector3.up;
            //アンカーを置く
            Cmd_rezobjectonserver(CreatePosition);
        
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
        flow.WhichCreatePlayer = isfrom;

        //流れのベクトルに合わせて回転させる
        float dist = Vector3.Distance(tpos, thisPositon);
        float leap = ((1.5f + dist) / dist) * 0.5f;//少し出す位置をずらす
        boxCol.transform.position = Vector3.Lerp(tpos, thisPositon, leap);
        boxCol.transform.rotation = Quaternion.FromToRotation(Vector3.up,flowvec.normalized);
        NetworkServer.Spawn(boxCol);
    }

    /// <summary>
    /// 現在未使用
    /// </summary>
    /// <returns></returns>
    bool IsPossibleCreateFlow()
    {
        //すり抜けチェックは使えるかも
        ////アピールエリアに繋ぐ流れは壁をすり抜けない
        //Ray ray = new Ray(appealArea.transform.position + Vector3.up, flowVector);
        //float radius = 1;
        //RaycastHit hit;
        //if(Physics.SphereCast(ray, radius, out hit))
        //{
        //    //あたったのが床だったらダメ
        //    if(hit.transform.gameObject.tag == "Plane")
        //        return false;
        //}

        return true;
    }

}

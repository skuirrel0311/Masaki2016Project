using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using GamepadInput;

public class PlayerCreateAnchor : NetworkBehaviour
{

    [SerializeField]
    GameObject InstanceAnchorHost;

    [SerializeField]
    GameObject InstanceAnchorClient;
    [SerializeField]
    float UncreateDistance = 3;

    [SerializeField]
    GameObject FlowEffect;

    new GameObject camera;

    private int playerNum;
    PlayerState playerState;

    GameObject cameraObj;
    GameObject targetAnchor = null;

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

    private float timer = -1;

    [SerializeField]
    private float ShotDistance = 1;

    [SerializeField]
    AudioClip createSE;
    [SerializeField]
    AudioClip missSE;
    AudioSource audioSource;

    List<GameObject> anchorQueueHost = new List<GameObject>();
    List<GameObject> anchorQueueClient = new List<GameObject>();

    [SerializeField]
    int maxAnchor = 5;

    // Use this for initialization
    void Start()
    {
        playerNum = GetComponentInParent<PlayerControl>().playerNum;
        camera = GameObject.Find("Camera1");
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("ThirdPersonCamera");
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        anchorQueueHost = new List<GameObject>();
        anchorQueueClient = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePadInput.GetTrigger(GamePadInput.Trigger.LeftTrigger, GamePadInput.Index.One) == 1.0f && timer == -1)
        {
            timer = 0;
            if (MainGameManager.IsPause) return;
            if (camera.GetComponent<CameraLockon>().IsLockOn == false)
            {
                audioSource.PlayOneShot(missSE);
                return;
            }
            camera.GetComponent<CameraLockon>().LockOnCut();
            Debug.Log("start");

            //始点を決める
            SetCreatePosition();

            //終点を決める
            GetTargetAnchor();

            //始点と終点の間に異物混入
            if (!IsPossibleCreateFlow())
            {
                audioSource.PlayOneShot(missSE);
                return;
            }

            //アンカーを置く
            CreateAnchor();

            //流れを生成する
            CmdCreateFlowObject(targetPosition, CreatePosition, flowVector, isServer);
        }

        if (timer != -1)
        {
            timer += Time.deltaTime;

            if (timer > ShotDistance)
            {
                timer = -1;
            }
        }
    }

    void GetTargetAnchor()
    {
        CameraLockon cameraLockon = camera.GetComponent<CameraLockon>();
        targetAnchor = cameraLockon.targetAnchor;
        float distance = 1000000;
        if (targetAnchor != null)
        {
            targetPosition = targetAnchor.transform.position;
            flowVector = targetPosition - CreatePosition;
            return;
        }

        targetAnchor = cameraLockon.GetTargetAnchor();
        if (targetAnchor != null)
        {
            targetPosition = targetAnchor.transform.position;
            flowVector = targetPosition - CreatePosition;
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

    void SetCreatePosition()
    {
        //カメラの向いている方向にプレイヤーを向ける
        float rotationY = cameraObj.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        CreatePosition = transform.position + Vector3.up;
    }

    [ClientCallback]
    void CreateAnchor()
    {
        //アンカーを置く
        Cmd_rezobjectonserver(CreatePosition, isServer);
        audioSource.PlayOneShot(createSE);
        Debug.Log("clientCallend");
    }

    [Command]
    public void Cmd_rezobjectonserver(Vector3 createPosition, bool isCreater)
    {
        Debug.Log("end1");
        GameObject obj;
        if (isCreater)
        {
            obj = Instantiate(InstanceAnchorHost, createPosition, transform.rotation) as GameObject;
            anchorQueueHost.Add(obj);
            anchorQueueHost.RemoveAll(n=>n==null);
            
        }
        else
        {
            obj = Instantiate(InstanceAnchorClient, createPosition, transform.rotation) as GameObject;
            anchorQueueClient.Add(obj);
            anchorQueueClient.RemoveAll(n => n == null);
        }

        

        if (anchorQueueHost.Count > maxAnchor)
        {
            anchorQueueHost[0].GetComponent<AnchorHit>().Crush();
            anchorQueueHost.Remove(anchorQueueHost[0]);
        }

        if (anchorQueueClient.Count > maxAnchor)
        {
            anchorQueueClient[0].GetComponent<AnchorHit>().Crush();
            anchorQueueClient.Remove(anchorQueueClient[0]);
        }

        obj.GetComponent<CreateFlow>().SetCreatePlayerIndex(1);
        NetworkServer.Spawn(obj);
        Debug.Log("end2");
    }

    [Command]
    void CmdCreateFlowObject(Vector3 tpos, Vector3 thisPositon, Vector3 flowvec, bool isfrom)
    {
        if (isfrom == false)
            Debug.Log("Create Client Flow");
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
        boxCol.transform.rotation = Quaternion.FromToRotation(Vector3.up, flowvec.normalized);
        NetworkServer.Spawn(boxCol);
    }

    /// <summary>
    /// 流れを繋いでも大丈夫か？(RayCastをしてBox,Planeがあったらfalseを返す)
    /// </summary>
    bool IsPossibleCreateFlow()
    {
        Ray ray = new Ray(CreatePosition, flowVector);
        float radius = 0.1f;
        List<GameObject> hits = Physics.SphereCastAll(ray, radius, flowVector.magnitude).Select(element => element.transform.gameObject).ToList();

        foreach (GameObject hit in hits)
        {
            if (hit.tag == "Box" || hit.tag == "Plane") return false;
        }

        return true;
    }

    public static bool IsPossibleCreateFlow(Vector3 position, Vector3 flowVec)
    {
        Ray ray = new Ray(position, flowVec);
        float radius = 0.1f;
        List<GameObject> hits = Physics.SphereCastAll(ray, radius, flowVec.magnitude).Select(element => element.transform.gameObject).ToList();

        foreach (GameObject hit in hits)
        {
            if (hit.tag == "Box" || hit.tag == "Plane") return false;
        }

        return true;
    }

}

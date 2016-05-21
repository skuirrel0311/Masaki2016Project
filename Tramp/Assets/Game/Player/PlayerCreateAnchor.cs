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
    GameObject targetAnchor=null;
    Vector3 flowVector;
    Vector3 targetPosition;
    Vector3 CreatePosition;
    float collsionRadius = 1;

    // Use this for initialization
    void Start()
    {
        playerNum = GetComponentInParent<PlayerControl>().playerNum;
        camera = GameObject.Find("Camera1");
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePadInput.GetTrigger(GamePadInput.Trigger.LeftTrigger,GamePadInput.Index.One)==1.0f&&camera.GetComponent<CameraControl>().IsLockOn)
        {
            if (MainGameManager.IsPause) return;
            if (CheckNearAnchor())
            {
                camera.GetComponent<CameraControl>().IsLockOn = false;
                Debug.Log("start");
                CreateAnchor();

                GetTargetAnchor();

                CmdCreateFlowObject(targetPosition, CreatePosition,flowVector);
            }
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
        CreatePosition = transform.position + transform.forward * 2 + Vector3.up;

        Cmd_rezobjectonserver();
        Debug.Log("clientCallend");
    }

    [Command]
    public void Cmd_rezobjectonserver()
    {
        Debug.Log("end1");
        GameObject obj;
        obj = Instantiate(InstanceAnchor,transform.position + transform.forward * 2 + Vector3.up, transform.rotation) as GameObject;
        obj.GetComponent<CreateFlow>().SetCreatePlayerIndex(1);
        NetworkServer.Spawn(obj);
        Debug.Log("end2");
    }

    [Command]
    void CmdCreateFlowObject(Vector3 tpos,Vector3 thisPositon, Vector3 flowvec)
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
        //流れのベクトルに合わせて回転させる
        float dist = Vector3.Distance(tpos, thisPositon);
        float leap = ((1.5f + dist) / dist) * 0.5f;//少し出す位置をずらす
        boxCol.transform.position = Vector3.Lerp(tpos, thisPositon, leap);
        boxCol.transform.rotation = Quaternion.FromToRotation(Vector3.up,flowvec.normalized);
        NetworkServer.Spawn(boxCol);
    }

}

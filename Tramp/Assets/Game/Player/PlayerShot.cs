﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GamepadInput;
using XInputDotNetPure;

public class PlayerShot : NetworkBehaviour
{
    [SerializeField]
    GameObject Ammo;

    GameObject cameraObj;
    Camera cam;

    private PlayerState playerState;
    private int playerNum;

    /// <summary>
    /// 弾の発射される位置
    /// </summary>
    [SerializeField]
    private Transform shotPosition;

    /// <summary>
    /// 弾の発射間隔
    /// </summary>
    [SerializeField]
    float shotDistance = 0.2f;

    /// <summary>
    /// 弾の残弾数
    /// </summary>
    [SerializeField]
    int stockMax = 30;
    [SerializeField]
    int stock;
    //弾を連射中か
    bool isShot;

    /// <summary>
    /// リロードする必要があるか？
    /// </summary>
    bool IsReload;

    GamepadInputState padState;

    void Start()
    {
        playerNum = GetComponent<PlayerControl>().playerNum;
        playerState = GetComponent<PlayerState>();
        cameraObj = GameObject.Find("Camera" + playerNum);
        cam = cameraObj.GetComponentInChildren<Camera>();
        StartCoroutine("LongTriggerDown");
        stock = stockMax;
        IsReload = false;
        isShot = false;
        vibrationTimer = 0;
    }

    float vibrationTimer;

    void Shot()
    {

        playerState.animator.SetLayerWeight(1, 1);

        GamePadState padState = GamePad.GetState(PlayerIndex.One);

        if (vibrationTimer == -1)
        {
            GamePad.SetVibration(PlayerIndex.One, 0.2f, 0.2f);
            vibrationTimer = 0;
        }


        //ストックを減らす
        stock--;
        
        Vector3 targetPosition = GetTargetPosition();

        //先にプレイヤーをカメラと同じ方向に向ける
        Quaternion cameraRotation = cameraObj.transform.rotation;
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        if (!isShot)
            transform.rotation = cameraRotation;

        isShot = true;

        shotPosition.LookAt(targetPosition);

        if (isServer)
        {
            GameObject go = Instantiate(Ammo, shotPosition.position, shotPosition.rotation) as GameObject;
            go.GetComponent<Shot>().isLocal=isLocalPlayer;
            NetworkServer.Spawn(go);
        }
        else
        {
            GameObject go = Instantiate(Ammo, shotPosition.position, shotPosition.rotation) as GameObject;
            go.GetComponent<Shot>().isLocal = isLocalPlayer;
            CmdAmmoSpawn(shotPosition.position, shotPosition.rotation);
        }
    }

    void Update()
    {
        if (GamePadInput.GetTrigger(GamePadInput.Trigger.RightTrigger, (GamePadInput.Index)playerNum, true) <= 0 && isLocalPlayer)
        {
            playerState.animator.SetBool("RunShotEnd", true);
            isShot = false;
        }

        if (vibrationTimer != -1)
        {
            vibrationTimer += Time.deltaTime;
            if (vibrationTimer > 0.05f)
            {
                vibrationTimer = -1;
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }
        }
    }

    [Command]
    public void CmdAmmoSpawn(Vector3 shotposition, Quaternion shotrotation)
    {
        GameObject go = Instantiate(Ammo, shotposition, shotrotation) as GameObject;
        go.transform.rotation = shotrotation;
    }

    IEnumerator LongTriggerDown()
    {
        while (true)
        {

            if (IsReload)
            {
                while (true)
                {
                    if (!GamePadInput.GetButtonDown(GamePadInput.Button.X, (GamePadInput.Index)playerNum)) yield return null;
                    else break;
                }
                //ボタン押したら3秒待つ
                gameObject.GetComponentInChildren<Animator>().SetBool("Reload", true);
                //動けない
                GetComponent<PlayerControl>().enabled = false;
                yield return new WaitForSeconds(3);

                //リロードが終わったら
                IsReload = false;
                gameObject.GetComponentInChildren<Animator>().SetBool("Reload", false);
                GetComponent<PlayerControl>().enabled = true;
                stock = stockMax;
            }

            if (GamePadInput.GetTrigger(GamePadInput.Trigger.RightTrigger, (GamePadInput.Index)playerNum, true) > 0 && !playerState.IsAppealing && isLocalPlayer)
            {
                Shot();

                playerState.animator.SetBool("RunShotEnd", false);
            }
            else
                yield return null;



            //ストックが無くなった
            if (stock < 0)
            {
                IsReload = true;
            }

            yield return new WaitForSeconds(shotDistance);
        }
    }

    Vector3 GetTargetPosition()
    {
        if (LookPlayer()) return GetAdversary().transform.position + Vector3.up;

        //カメラの中心座標からレイを飛ばす
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {

            Vector3 temp = hit.point - shotPosition.position;
            float angle = Vector3.Angle(ray.direction, temp);

            if (angle < 90) return hit.point;
            else return ray.origin + (ray.direction * 100);
        }
        else return ray.origin + (ray.direction * 100);
    }

    /// <summary>
    /// 対戦相手が見えるかどうか？
    /// </summary>
    bool LookPlayer()
    {
        GameObject obj = GetAdversary();

        if (obj == null) return false;

        Vector3 toPlayerVector = obj.transform.position - cameraObj.transform.position;
        Vector3 cameraForward = cameraObj.transform.forward;

        float verticalAngle = VerticalAngle(toPlayerVector, cameraObj.transform.forward);
        verticalAngle = Mathf.Abs(verticalAngle);
        float horizontalAngle = Vector2.Angle(new Vector2(toPlayerVector.x, toPlayerVector.z),
            new Vector2(cameraForward.x, cameraForward.z));

        return verticalAngle < 5 && horizontalAngle < 5;
    }

    /// <summary>
    /// 縦方向のAngleを返す
    /// </summary>
    float VerticalAngle(Vector3 vec1, Vector3 vec2)
    {
        //X方向だけのベクトルに変換
        Vector3 temp1 = Vector3.right * vec1.magnitude;
        Vector3 temp2 = Vector3.right * vec2.magnitude;

        //Y座標を代入
        temp1.y = vec1.y;
        temp2.y = vec2.y;

        //それぞれの角度を求める
        float rot1 = Mathf.Atan2(temp1.y, temp1.x) * Mathf.Rad2Deg;
        float rot2 = Mathf.Atan2(temp2.y, temp2.x) * Mathf.Rad2Deg;

        return rot1 - rot2;
    }

    /// <summary>
    /// 対戦相手を取得します
    /// </summary>
    GameObject GetAdversary()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in players)
        {
            if (obj.Equals(gameObject)) continue;
            return obj;
        }
        return null;
    }

    public float testnum=0;
    void OnAnimatorIK(int layerIndex)
    {
        if (isShot)
        {

            Vector3 localAngles = playerState.animator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;

            //先にプレイヤーをカメラと同じ方向に向ける
            Vector3 cameraRotation = cameraObj.transform.eulerAngles;

            cameraRotation=cameraRotation - transform.rotation.eulerAngles;

            playerState.animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(cameraRotation.y, 180.0f,-cameraRotation.x+180.0f));
        }

        else
        {
            playerState.animator.SetLayerWeight(layerIndex, 0);
        }
    }

    void OnGUI()
    {
        if (!IsReload) return;

        GUI.TextField(new Rect(500, 100, 100, 30), "Push_X Reload");
    }

}

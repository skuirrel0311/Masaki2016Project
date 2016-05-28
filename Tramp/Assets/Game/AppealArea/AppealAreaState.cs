using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class AppealAreaState : NetworkBehaviour
{
    //占有度
    [SyncVar]
    private float share;

    //どちらが占領中か
    [SyncVar]
    private bool isOccupiers;

    private List<GameObject> RidePlayers = new List<GameObject>();

    private GameObject ShareImage;

    void Start()
    {
        isOccupiers = false;
        share = 0;
        RidePlayers = new List<GameObject>();
        ShareImage = GameObject.Find("ShareImage");
    }

    void Update()
    {
        //占有度のアップデート
        UpdateShare();

        //乗っている間の表示処理
        ShareUI();
    }

    //占有度に変化があるときの処理
    void UpdateShare()
    {
        if (RidePlayers.Count != 1) return;

        //誰にも占拠されていない
        if (share == 0)
        {
            CmdChangeOccupiers(isServer);
            CmdChangeShare(1);
        }
        //自分が占拠している
        else if (isOccupiers == isServer)
        {
            CmdChangeShare(1);
        }
        //相手に占拠されている
        else
        {
            CmdChangeShare(-1);
        }

    }

    void ShareUI()
    {
        bool localPlayer=false;
        foreach (GameObject player in RidePlayers)
        {
            if (player.GetComponent<PlayerState>().isLocalPlayer)
            {
                ShareImage.SetActive(true);
                ShareImage.GetComponent<Image>().fillAmount = share / 100;
                localPlayer = true;
            }
        }

        if(localPlayer==false)
        {
            ShareImage.SetActive(false);
        }
    }

    [Command]
    void CmdChangeOccupiers(bool isSever)
    {
        isOccupiers = isServer;
    }

    [Command]
    void CmdChangeShare(float value)
    {
        share += value;
        if (share >= 100)
        {
            share = 100;
        }
        else if (share <= 0)
        {
            share = 0;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player") return;
        RidePlayers.Add(col.gameObject);
    }

    void OnTirggerExit(Collider col)
    {
        if (col.tag != "Player") return;
        RidePlayers.Remove(col.gameObject);
    }
}

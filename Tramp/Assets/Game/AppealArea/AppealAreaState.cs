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

    //占領済みか
    [SyncVar]
    public bool isOccupation;

    private List<GameObject> RidePlayers = new List<GameObject>();

    private GameObject ShareImageHost;
    private GameObject ShareImageClient;

    [SerializeField]
    private Image ShareImage;

    private static bool isDrawUI=false;

    void Awake()
    {
        ShareImageHost = GameObject.Find("ShareImageHost");
        ShareImageClient = GameObject.Find("ShareImageClient");
    }

    void Start()
    {
        isOccupiers = false;
        isOccupation = false;
        share = 0;
        RidePlayers = new List<GameObject>();
    }

    void FixedUpdate()
    {
        isDrawUI = false;
    }

    void Update()
    {
        //占有度のアップデート
        UpdateShare();

        //乗っている間の表示処理
        ShareUI();
    }

    void LateUpdate()
    {
        if (isDrawUI == false)
        {
            ShareImageHost.SetActive(false);
            ShareImageClient.SetActive(false);
        }
    }

    //占有度に変化があるときの処理
    void UpdateShare()
    {
        if (RidePlayers.Count != 1) return;
        if (!isServer) return;

        //誰にも占拠されていない
        if (share == 0)
        {
            ChangeOccupiers(RidePlayers[0].GetComponent<PlayerState>().isLocalPlayer);
            CmdChangeShare(1);
        }
        //自分が占拠している
        else if (isOccupiers == RidePlayers[0].GetComponent<PlayerState>().isLocalPlayer)
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
        if (isOccupiers == true)
        {
            ShareImage.color = Color.white;
            ShareImage.fillAmount = share / 100;
        }
        else
        {
            ShareImage.color = Color.red;
            ShareImage.fillAmount = share / 100;
        }

        foreach (GameObject player in RidePlayers)
        {
            if (player.GetComponent<PlayerState>().isLocalPlayer)
            {
                if (isOccupiers == true)
                {
                    ShareImageHost.SetActive(true);
                    ShareImageHost.GetComponent<Image>().fillAmount = share / 100;
                }
                else
                {
                    ShareImageClient.SetActive(true);
                    ShareImageClient.GetComponent<Image>().fillAmount = share / 100;
                }
                isDrawUI = true;
            }
        }
    }

    [Client]
    void ChangeOccupiers(bool isSev)
    {
        if (isSev)
        {
            CmdChangeSeverOccupiers();
        }
        else
        {
            CmdChangeClinetOccupiers();
        }
    }

    [Command]
    void CmdChangeSeverOccupiers()
    {
        isOccupiers = true;
    }

    [Command]
    void CmdChangeClinetOccupiers()
    {
        isOccupiers = false;
    }

    [Command]
    void CmdChangeShare(float value)
    {
        share += value;
        if (share >= 100)
        {
            share = 100;
            isOccupation = true;
        }
        else if (share <= 0)
        {
            share = 0;
            isOccupation = false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player") return;
        RidePlayers.Add(col.gameObject);
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag != "Player") return;
        RidePlayers.Remove(col.gameObject);
    }
}

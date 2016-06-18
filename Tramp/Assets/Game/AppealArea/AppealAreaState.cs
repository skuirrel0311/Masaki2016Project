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
    public bool isOccupiers;

    //占領済みか
    [SyncVar]
    public bool isOccupation;

    public List<GameObject> RidePlayers = new List<GameObject>();

    private GameObject ShareImageHost;
    private GameObject ShareImageClient;

    [SerializeField]
    private Texture SeverTexture;

    [SerializeField]
    private Texture ClientTexture;

    [SerializeField]
    private Texture NeutralTexture;

    [SerializeField]
    private GameObject AreaEffect;

    [SerializeField]
    private GameObject ShareImageObject;

    private Image ShareImage;

    Renderer StageMesh;

    private static bool isDrawUI = false;
    private MainGameManager mainManager;
    private MyNetworkManager myNetManager;

    [SerializeField]
    AudioClip occupiersSE;
    [SerializeField]
    AudioClip completeSE;

    AudioSource audioSource;
    public AudioSource loopAudioSource;

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
        StageMesh = transform.FindChild("pSphere1").GetComponent<Renderer>();
        ShareImage = ShareImageObject.transform.FindChild("ShareImage").GetComponent<Image>();
        ShareImageObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.position.x, transform.position.z);
        mainManager = GameObject.Find("MainGameManager").GetComponent<MainGameManager>();
        myNetManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        loopAudioSource = GetComponent<AudioSource>();
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

        if (isOccupation)
        {
            if (isOccupiers)
            {
                StageMesh.materials[0].mainTexture = SeverTexture;
            }
            else
            {
                StageMesh.materials[0].mainTexture = ClientTexture;
            }
        }
        else
        {
            StageMesh.materials[0].mainTexture = NeutralTexture;
        }
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
        //サーバー
        else if (RidePlayers[0].GetComponent<PlayerState>().isLocalPlayer)
        {
            if (isOccupiers)
            {
                CmdChangeShare((float)(1.0f+(Mathf.Max(myNetManager.occupied, 0) / 1.0f)));
            }
            else
            {
                CmdChangeShare((float)(-1.0f - (Mathf.Max(myNetManager.occupied, 0) / 1.0f)));
            }
        }
        //クライアント
        else
        {
            if (!isOccupiers)
            {
                CmdChangeShare((float)(1.0f+(Mathf.Max(myNetManager.occuping, 0) / 1.0f)));
            }
            else
            {
                CmdChangeShare((float)(-1.0f- (Mathf.Max(myNetManager.occuping, 0) /  1.0f)));
            }
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
            if (!isOccupation)
                RpcAreaEffect();
            isOccupation = true;
        }
        else if (share <= 0)
        {
            share = 0;
            if (isOccupation)
                RpcAreaEffect();
            isOccupation = false;
        }
    }

    [ClientRpc]
    void RpcAreaEffect()
    {
        Instantiate(AreaEffect, transform.position, transform.rotation);
        loopAudioSource.Stop();
        audioSource.PlayOneShot(completeSE);
    }
}

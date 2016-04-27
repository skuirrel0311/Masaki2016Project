using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class FeverGauge : NetworkBehaviour
{
    [SerializeField]
    Image gaugeSprite;//中身
    float gaugeMaxWidth = 1.0f; //中身のスプライトが満タンになる拡大率
    int gaugeMaxPoint = 100;
    /// <summary>
    /// 現在のフィーバーポイント
    /// </summary>
    [SerializeField]
    [SyncVar]
    public int feverPoint;

    int oppnentFeverGauge;

    /// <summary>
    /// 敵を倒したときに上昇する量
    /// </summary>
    int killPoint = 10;
    /// <summary>
    /// 敵に倒されたときに減少する量
    /// </summary>
    int killedPoint = -5;


    void Start()
    {
        oppnentFeverGauge = 0;
        feverPoint = 20;
        if (isServer)
        {
            if (isLocalPlayer)
                gaugeSprite = GameObject.Find("Gauge1").GetComponent<Image>();
            else
                gaugeSprite = GameObject.Find("Gauge2").GetComponent<Image>();
        }
        else
        {
            if (isLocalPlayer)
                gaugeSprite = GameObject.Find("Gauge2").GetComponent<Image>();
            else
                gaugeSprite = GameObject.Find("Gauge1").GetComponent<Image>();
        }
    }


    void Update()
    {


        float width = (float)feverPoint / (feverPoint+oppnentFeverGauge);
        width = gaugeMaxWidth * width;

        gaugeSprite.transform.localScale = new Vector3(width, 1, 1);

        if (Input.GetKey(KeyCode.A) && isLocalPlayer)
            CmdAddPoint(10);

        GetOppnentFeverGauge();

    }

    void GetOppnentFeverGauge()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Player");
        if (go.Length <= 1) return;

        for (int i = 0; i < go.Length; i++)
        {
            if (go[i]!=gameObject)
            {
                oppnentFeverGauge = go[i].GetComponent<FeverGauge>().feverPoint;
            }
        }
    }

    /// <summary>
    /// 渡された数値分ゲージが増減します。
    /// </summary>
    [Command]
    public void CmdAddPoint(int num)
    {
        feverPoint += num;
        if (feverPoint < 0) feverPoint = 0;
        if (feverPoint > gaugeMaxPoint) feverPoint = gaugeMaxPoint;
    }

    /// <summary>
    /// プレイヤーを倒したときに呼んでください
    /// </summary>
    public void KillPlayer()
    {
        CmdAddPoint(killPoint);
    }

    /// <summary>
    /// プレイヤーに殺されたときに呼んでください
    /// </summary>
    public void KilledInPlayer()
    {
        CmdAddPoint(killedPoint);
    }


}

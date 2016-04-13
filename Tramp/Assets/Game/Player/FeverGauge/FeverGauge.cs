using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeverGauge : MonoBehaviour
{
    [SerializeField]
    Image gaugeSprite;//中身
    float gaugeMaxWidth = 3.82f; //中身のスプライトが満タンになる拡大率
    int gaugeMaxPoint = 100;
    /// <summary>
    /// 現在のフィーバーポイント
    /// </summary>
    public int FeverPoint { get; private set; }

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
        FeverPoint = 20;
    }

    void Update()
    {
        float width =(float)FeverPoint / gaugeMaxPoint;
        width = gaugeMaxWidth * width;
        
        gaugeSprite.transform.localScale = new Vector3(width, 1, 1);
    }

    /// <summary>
    /// 渡された数値分ゲージが増減します。
    /// </summary>
    public void AddPoint(int num)
    {
        FeverPoint += num;
        if (FeverPoint < 0) FeverPoint = 0;
        if (FeverPoint > gaugeMaxPoint) FeverPoint = gaugeMaxPoint;
    }

    /// <summary>
    /// プレイヤーを倒したときに呼んでください
    /// </summary>
    public void KillPlayer()
    {
        AddPoint(killPoint);
    }

    /// <summary>
    /// プレイヤーに殺されたときに呼んでください
    /// </summary>
    public void KilledInPlayer()
    {
        AddPoint(killedPoint);
    }

}

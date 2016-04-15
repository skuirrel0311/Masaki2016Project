using UnityEngine;
using System.Collections.Generic;

public class PlayerAppeal : MonoBehaviour
{
    [SerializeField]
    ParticleSystem starParticle;

    FeverGauge feverGauge;

    PlayerState playerState;

    /// <summary>
    /// アピール中に踏破したエリア
    /// </summary>
    List<GameObject> ChackedArea = new List<GameObject>();

    void Start()
    {
        feverGauge = GetComponent<FeverGauge>();
        playerState = GetComponent<PlayerState>();
    }
    
    void Update()
    {
        if (playerState.IsAppealing) Appeal();

        //エリアが踏破されているのにアピールしていなかったら
        if(ChackedArea.Count != 0 && playerState.IsAppealing == false)
        {
            EndAppeal();
        } 
    }

    /// <summary>
    /// アピールが始まる瞬間
    /// </summary>
    void StartAppeal()
    {
        //流れのパーティクルをインスタンス、子のオブジェクトとして追加
        ParticleSystem particle = (ParticleSystem)Instantiate(starParticle, transform.position, transform.rotation);
        particle.transform.parent = transform;
        particle.Play();
    }

    void Appeal()
    {
        //常に１ポイント付与
        feverGauge.AddPoint(1);
    }

    /// <summary>
    /// アピールが終わった瞬間
    /// </summary>
    void EndAppeal()
    {
        starParticle.Stop();

        float additional; //加点

        additional = ChackedArea.Count * (ChackedArea.Count * 1.2f);

        feverGauge.AddPoint((int)additional);

        //リストを破棄
        //ChackedArea.RemoveAll(n => true);
        for(int i = ChackedArea.Count; i > 0; i--)
        {
            ChackedArea[i] = null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (playerState.IsAppealing == false) return;
        if (col.gameObject.tag != "Area") return;

        //初回はStartAppealを呼ぶ
        if (ChackedArea.Count == 0) StartAppeal();

        //すでに踏破されていたらreturn
        foreach(GameObject g in ChackedArea)
        {
            if (g.Equals(col.gameObject)) return;
        }

        ChackedArea.Add(col.gameObject);
    }
}

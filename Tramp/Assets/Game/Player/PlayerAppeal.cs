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
        Vector3 particlePosition = transform.position;
        particlePosition.y += 1;

        starParticle = (ParticleSystem)Instantiate(starParticle, particlePosition, transform.rotation);
        starParticle.transform.parent = transform;
        
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
        Debug.Log("startParticle");
        starParticle.Play();
    }

    void Appeal()
    {
        //常に１ポイント付与
        feverGauge.CmdAddPoint(1);
    }

    /// <summary>
    /// アピールが終わった瞬間
    /// </summary>
    void EndAppeal()
    {
        starParticle.Stop();

        float additional; //加点

        additional = 100 * (ChackedArea.Count * 1.2f);

        feverGauge.CmdAddPoint((int)additional);

        //リストを破棄
        ChackedArea.Clear();
    }

    void OnTriggerEnter(Collider col)
    {
        if (playerState.IsAppealing == false) return;
        if (col.gameObject.tag != "Area") return;
        if (ChackedArea.Count == 0) return;

        //すでに踏破されていたらreturn
        foreach(GameObject g in ChackedArea)
        {
            if (g.Equals(col.gameObject)) return;
        }

        ChackedArea.Add(col.gameObject);
    }

    void OnTriggerStay(Collider col)
    {
        if (playerState.IsAppealing == false) return;
        if (col.gameObject.tag != "Area") return;
        if (ChackedArea.Count != 0) return;

        //初回のみStayで判定

        StartAppeal();
        ChackedArea.Add(col.gameObject);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HpGauge : MonoBehaviour
{
    //ゲージの枠
    GameObject gaugeLim;

    [SerializeField]
    Sprite lim;

    [SerializeField]
    Sprite deadLim;

    //ゲージの中身
    GameObject[] aliveGaugePoint;

    [SerializeField]
    Sprite alivePoint;

    PlayerState playerState;



    void Start()
    {
        playerState = GetComponent<PlayerState>();
        if (!playerState.isLocalPlayer) return;
        aliveGaugePoint = new GameObject[playerState.maxHp];
        for (int i = 0; i < aliveGaugePoint.Length; i++) aliveGaugePoint[i] = GameObject.Find("HPoint" + (i + 1).ToString());
        gaugeLim = GameObject.Find("HpGaugeLim");
        gaugeLim.GetComponent<Image>().sprite = lim;

        for (int i = 0; i < aliveGaugePoint.Length; i++){
            aliveGaugePoint[i].GetComponent<Image>().sprite = alivePoint;
        }
    }

    public void HitPointUI(int hp)
    {
        if (!playerState.isLocalPlayer) return;

        bool[] life;

        life = new bool[playerState.maxHp];

        gaugeLim.GetComponent<Image>().sprite = hp != 0 ? lim : deadLim;

        for (int i = 0; i < hp; i++) life[i] = true;

        for (int i = 0; i < aliveGaugePoint.Length; i++)
        {
            aliveGaugePoint[i].SetActive(life[i]);
        }
    }
}

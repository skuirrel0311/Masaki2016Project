using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GunGauge : MonoBehaviour
{
    [SerializeField]
    Image gunEnergy;

    PlayerShot shot;

    //三日月形なのでかけている部分がある
    const float gaugeMax = 0.81f;

    void Start()
    {
        shot = GetComponent<PlayerShot>();
    }

    void Update()
    {
        int max = shot.StockMax;
        int stock = shot.Stock;

        //割合を求める
        float num = (float)stock / max;

        num = Mathf.Lerp(0, 0.81f, num);

        gunEnergy.fillAmount = num;

    }
}

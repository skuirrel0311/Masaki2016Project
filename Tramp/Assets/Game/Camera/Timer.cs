using UnityEngine;

public class Timer : MonoBehaviour
{
    float time;

    float limitTime;

    bool IsWorking;

    /// <summary>
    /// 進捗(0～1)
    /// </summary>
    public float Progress { get { return time / limitTime; } }
    
    /// <summary>
    /// 指定した時間を過ぎた
    /// </summary>
    public bool IsLimitTime { get; private set; }

    /// <summary>
    /// タイマーを起動させます
    /// </summary>
    public void TimerStart(float limitTime)
    {
        time = 0;
        this.limitTime = limitTime;
        IsWorking = true;
        IsLimitTime = false;
    }

    /// <summary>
    /// タイマーを一時停止させます。
    /// </summary>
    public void Stop()
    {
        IsWorking = false;
    }
    
    public void Update()
    {
        if (!IsWorking) return;
        time = Mathf.Min(time + Time.deltaTime, limitTime);

        if (time == limitTime) IsLimitTime = true;
    }
}

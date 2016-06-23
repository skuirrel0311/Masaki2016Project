using UnityEngine;

public class Timer : MonoBehaviour
{
    float time;

    float limitTime;

    public bool IsWorking { get; private set; }

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
    public void TimerStart(float limitTime, bool IsLimitTime = false)
    {
        time = 0;
        this.limitTime = limitTime;
        IsWorking = true;
        this.IsLimitTime = IsLimitTime;
    }

    /// <summary>
    /// タイマーの時間をリセットします(起動はしたまま)
    /// </summary>
    public void Reset()
    {
        time = 0;
        IsLimitTime = false;
    }

    /// <summary>
    /// タイマーを一時停止させます。
    /// </summary>
    public void Stop(bool isReset = false)
    {
        IsWorking = false;
        if (isReset) Reset();
    }
    
    public void Update()
    {
        if (!IsWorking) return;
        time = Mathf.Min(time + Time.deltaTime, limitTime);

        if (time == limitTime) IsLimitTime = true;
    }
}

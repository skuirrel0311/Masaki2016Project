using UnityEngine;
using System.Collections;

public abstract class State<T>
{
    /// <summary>
    /// ステートの所持者
    /// </summary>
    protected T owner;

    public State(T owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// この状態になったときに1度だけ呼ばれる
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// この状態のとき毎フレーム呼ばれる
    /// </summary>
    public virtual void Execute() { }

    /// <summary>
    /// この状態から別のステートになったときに1度だけ呼ばれる
    /// </summary>
    public virtual void Exit() { }
}

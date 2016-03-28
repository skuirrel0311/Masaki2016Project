using UnityEngine;
using System.Collections;

public class State<T>
{
    protected T owner;

    public State(T owner)
    {
        this.owner = owner;
    }

    //この状態になったときに1度だけ呼ばれる
    public virtual void Enter() { }

    //この状態のとき毎フレーム呼ばれる
    public virtual void Execute() { }

    //この状態から別のステートになったときに1度だけ呼ばれる
    public virtual void Exit() { }
}

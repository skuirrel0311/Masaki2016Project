using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyBase<T, Tenum> : MonoBehaviour
    where T : class where Tenum : System.IConvertible
{
    protected List<State<T>> stateList = new List<State<T>>();

    protected StateManager<T> stateManager;


    public virtual void ChangeState(Tenum state)
    {
        if (stateManager == null) return;

        stateManager.ChangeState(stateList[state.ToInt32(null)]);
    }

    public virtual bool IsCurrentstate(Tenum state)
    {
        if (stateManager == null) return false;

        return stateManager.CurrentState == stateList[state.ToInt32(null)];
    }

    protected virtual void Update()
    {
        if (stateManager == null) return;

        stateManager.Update();
    }
}

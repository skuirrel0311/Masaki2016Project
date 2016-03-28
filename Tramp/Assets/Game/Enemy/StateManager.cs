using UnityEngine;
using System.Collections;

public class StateManager<T>
{
    //今の状態
    private State<T> currentState;

    public State<T> CurrentState { get { return currentState; } }

    void Start()
    {
        currentState = null;
    }

    public void Update()
    {
        if(currentState == null) return;

        currentState.Execute();
    }

    public void ChangeState(State<T> state)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = state;
        currentState.Enter();
    }
}

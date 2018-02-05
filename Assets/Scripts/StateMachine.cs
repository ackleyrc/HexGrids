using System;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    public State currentState;

    public void Init(State state)
    {
        currentState = state;
        currentState.Enter();
    }

    void Update ()
    {
        currentState.DoUpdate();
    }

    public void ChangeState (Type newStateType)
    {
        currentState.Exit();
        State newState = GetComponent(newStateType) as State;
        if (newState == null)
        {
            newState = gameObject.AddComponent(newStateType) as State;
        }
        currentState = newState;
        currentState.Enter();
    }

    public void ChangeState (State newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }
}
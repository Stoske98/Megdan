using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    public abstract void Enter(Unit unit);
    public abstract void Execute(Unit unit);
    public abstract void Exit(Unit unit);
}


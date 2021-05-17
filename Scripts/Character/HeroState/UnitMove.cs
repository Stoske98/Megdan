using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitMove : State
{
    public override void Enter(Unit unit)
    {
        unit.animator.SetBool("Move", true);
    }

    public override void Execute(Unit unit)
    {
        unit.MoveUnit(unit.desiredHex);
        
    }

    public override void Exit(Unit unit)
    {
        unit.animator.SetBool("Move", false);
    }
}





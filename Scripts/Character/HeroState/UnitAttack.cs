using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitAttack : State
{
    public override void Enter(Unit unit)
    {
        
    }

    public override void Execute(Unit unit)
    {
        if(unit.readyToAttack == true)
        {
            unit.Attack(unit.TargetedUnit);

        }
        
    }

   

    public override void Exit(Unit unit)
    {
        unit.animator.SetBool("Attack", false);
    }

    
}



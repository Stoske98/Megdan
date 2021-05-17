using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
   
    public abstract string AttackType { get; protected set; }
    public abstract string Type { get; protected set; }

    public abstract int UnitsPriority { get; protected set; } // 1 = king, 2 = unit, 3 neutral
    public abstract int Level { get; protected set; }

    public abstract CharacterStats Stats { get; protected set; }

    protected abstract float CalculateDamage(float damage);

    protected abstract float CalculateMagicDamage(float damage);

    public abstract void RecieveDmg(float damage);
    public abstract void RecieveMagicDmg(float damage);

    public abstract void IncreaseLevel();

    public abstract void MoveUnit(Hex hex);
    public abstract void Attack(Unit unit);

    public abstract void Ability();
 

    

}

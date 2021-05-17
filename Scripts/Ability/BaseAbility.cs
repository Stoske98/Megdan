using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbility 
{
    public abstract string Name { get; protected set; }
    public abstract Sprite Sprite { get; protected set; }
    public abstract string Descritption { get; protected set; }
    public abstract string AbilityType { get; protected set; } // Magical, Physical or pure dmg
    public abstract bool isPassive { get; protected set; }
    public abstract bool isTargetable { get; protected set; }

    public abstract bool freeDirection { get; protected set; }
    public abstract int CoolDown { get; protected set; }

    public abstract void IncreaseLevel();




}

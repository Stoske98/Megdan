using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : BaseAbility
{
    public override string Name { get; protected set; }
    public override Sprite Sprite { get; protected set; }
    public override string Descritption { get; protected set; }
    public override string AbilityType { get; protected set; }
    public override bool isPassive { get; protected set; } = false;
    public override bool isTargetable { get; protected set; } = false;
    public override int CoolDown { get; protected  set; } = 0;
    public override bool freeDirection { get; protected set; } = false;

    public int maxCoolDown;
    public float Quantity;
    public int Range;
    public int abilityLvl = 1;
    public bool HexIsTargetable = false;
    public bool isUsed = false;

   public void setPassive(bool passive)
    {
        this.isPassive = passive;
    }
    public void setCoolDown(int cd)
    {
        this.CoolDown = cd;
    }
    public int getCoolDown()
    {
        return this.CoolDown;
    }

    public Ability()
    {
    }
    
    public Ability(string name, Sprite icon, int cooldown, string description, string abilityType)
    {
        this.Name = name;
        this.Sprite = icon;
        this.Descritption = description;
        this.AbilityType = abilityType;
        this.maxCoolDown = cooldown;
    }
    public Ability(string name, int cooldown, string description, string abilityType)
    {
        this.Name = name;
        this.Descritption = description;
        this.AbilityType = abilityType;
        this.maxCoolDown = cooldown;
    }
    public Ability(string name, float quantity, int cooldown, string description, string abilityType, int range)
    {
        this.Name = name;
        this.Quantity = quantity;
        this.Descritption = description;
        this.AbilityType = abilityType;
        this.maxCoolDown = cooldown;
        this.Range = range;
    }

    public Ability(string name, int cooldown, string description, string abilityType, int range)
    {
        this.Name = name;
        this.Quantity = 0;
        this.Descritption = description;
        this.AbilityType = abilityType;
        this.maxCoolDown = cooldown;
        this.Range = range;
    }
    public Ability(string name, float quantity, int cooldown, string description, string abilityType,bool isTargetable, int range)
    {
        this.Name = name;
        this.Quantity = quantity;
        this.Descritption = description;
        this.AbilityType = abilityType;
        this.isTargetable = isTargetable;
        this.maxCoolDown = cooldown;
        this.Range = range;
    }

    public Ability(string name, float quantity, int cooldown, string description, string abilityType, bool isTargetable)
    {
        this.Name = name;
        this.Quantity = quantity;
        this.Descritption = description;
        this.AbilityType = abilityType;
        this.isTargetable = isTargetable;
        this.maxCoolDown = cooldown;
    }

    public virtual void useAbility()
    {
    }
    public virtual void useAbility(Unit unit)
    {

    }
    public virtual void useAbility(List<Unit> units)
    {

    }

    public virtual void useAbility(Hex hex)
    {
    }

    public override void IncreaseLevel()
    {
    }
}

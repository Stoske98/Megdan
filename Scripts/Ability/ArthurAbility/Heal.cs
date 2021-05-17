using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Ability
{

    
    public GameObject Particle;

    public const string name = "Heal";
    public const int cooldown = 2;
    
    public const string description = "Default";
    public const string abilityType = "Magic";
    public const bool istargatable = true;
    public const float quantity = 250;
    public const int range = 1;


    public Heal( GameObject particle) : base(name, quantity, cooldown, description, abilityType, istargatable, range)
    {
        this.Particle = particle;

    }

    public override void useAbility(Unit unit)
    {
        
        
            GameObject heal = GameObject.Instantiate(Particle, unit.transform.position, Quaternion.identity);
            GameObject.Destroy(heal, 3);

            if (unit.Stats.MaxHealth > unit.Stats.Health)
            {
                if (unit.Stats.MaxHealth < unit.Stats.Health + this.Quantity)
                {
                    unit.Stats.Health = unit.Stats.MaxHealth;
                    unit.healthBar.SetHealth(unit.Stats.Health);
                    GameManager.Instance.updateUnitStats(unit);
                }
                else
                {
                    unit.Stats.Health += this.Quantity;
                    unit.healthBar.SetHealth(unit.Stats.Health);
                    GameManager.Instance.updateUnitStats(unit);
                }
            }

            foreach (var neighbor in GameManager.Instance.hexMap.GetNeighbors(unit.Hex))
            {

                if (neighbor.unit != null && neighbor.unit.team != unit.team)
                {
                    neighbor.unit.RecieveMagicDmg(this.Quantity / 2);
                    GameManager.Instance.updateUnitStats(neighbor.unit);
                }
            }

    }

    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Range = 2;
                this.Quantity = 350;
                break;
            case 3:
                this.Range = 3;
                this.Quantity = 450;
                break;

            default:
                break;
        }
    }
}

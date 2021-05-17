
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MagicBall : Ability
{
    public Transform spawnPojectilPosition;
    public GameObject Projectil;

    public const string name = "Laguna Blade";
    public const int cooldown = 2;
    public const string description = "Default";
    public const string abilityType = "Magic";
    public const bool istargatable = true;
    public const float quantity = 500;
    public const int range = 3;

    public MagicBall(GameObject projectil, Transform spawnPos) : base( name, quantity,  cooldown, description,  abilityType, istargatable, range)
    {
        this.spawnPojectilPosition = spawnPos;
        this.Projectil = projectil;
        
    }

    public override void useAbility(Unit unit)
    {

        GameObject unitGo = GameObject.Instantiate(Projectil, spawnPojectilPosition.position, Quaternion.identity);
        BallGO ball = unitGo.GetComponent<BallGO>();
        ball.enemy = unit;
        ball.DamageToDeal = this.Quantity;
        ball.ps.Stop();
    }


    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Quantity = 750;
                break;
            case 3:
                this.Quantity = 1000;
                break;

            default:
                break;
        }
    }
}

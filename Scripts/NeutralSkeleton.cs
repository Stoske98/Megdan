using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralSkeleton : Unit
{
    private Vector3 V = Vector3.forward;
    
    private void Awake()
    {
        heroImage = Resources.Load<Sprite>("Skeletons/Skeletons");
    }
    public override int Level { get; protected set; } = 1;
    public override CharacterStats Stats { get; protected set; } = new CharacterStats
    {
        MaxHealth = 500,
        Health = 500,

        Damage = 40,

        Energy = 20,
        AttackRange = 1,

        Armor = 0,
        MagicResist = 0


    };

    public override string AttackType { get; protected set; } = "Melee";
    public override string Type { get; protected set; } = "Neutral";

    public override void IncreaseLevel()
    {
        this.Level++;
        CharacterStats newStats;
       switch (this.Level)
        {
            case 2:
                newStats = new CharacterStats
                {
                    MaxHealth = 500,
                    Health = 500,

                    Damage = 80,

                    Energy = 20,
                    AttackRange = 1,

                    Armor = 0,
                    MagicResist = 0
                };
                Stats = newStats;
                break;

            case 3:
                newStats = new CharacterStats
                {
                    MaxHealth = 500,
                    Health = 500,

                    Damage = 120,

                    Energy = 20,
                    AttackRange = 1,

                    Armor = 0,
                    MagicResist = 0
                };
                Stats = newStats;
                break;

        }

    }

    public override void setEndOfAttack()
    {
        if (Stats.Energy != 0 )
        {
            this.TargetedUnit.RecieveDmg(this.Stats.Damage);
            Stats.Energy -= 1;

        }
        else
        {
            this.animator.SetBool("Attack", false);
            this.TargetedUnit = null;
            ChangeState(IdleState);
        }
        
    }
    public override void Attack(Unit unit)
    {

        if (readyToAttack)
        {
            this.gameObject.transform.LookAt(unit.gameObject.transform);
            this.animator.Play("Attack");
            readyToAttack = false;
        }

    }
    public override void MoveUnit(Hex end)
    {
        
        if (Path == null)
        {

            Path = PathFinder.FindPath_AStar(GameManager.Instance.hexMap, Hex, end).ToArray();
            if (Path.Length - 1 > this.Stats.Energy)
            {
                Path = null;
                ChangeState(IdleState);


            }
            else
            {
                end.Walkable = false;
                Hex.unit = null;
                pathIndex += 1;
            }
        }

        if (Path != null)
        {

            if (pathIndex < Path.Length)
            {
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(GameManager.Instance.hexMap.positionHex(Path[pathIndex]) - this.transform.position), 300 * Time.deltaTime);
                this.transform.position = Vector3.SmoothDamp(this.transform.position, GameManager.Instance.hexMap.positionHex(Path[pathIndex]), ref V, 0.2f);
                
                Hex = Path[pathIndex];
                if (pathIndex == 1 && Path[pathIndex].PrevTile != null)
                {
                    Path[pathIndex].PrevTile.Walkable = true;
                    Path[pathIndex].PrevTile.SetColor(GameManager.Instance.hexMap.TileColor_Default);
                }




            }

            if (Vector3.Distance(this.transform.position, GameManager.Instance.hexMap.positionHex(Path[pathIndex])) < 0.01f)
            {
                if (end == Path[pathIndex])
                {
                    Stats.Energy -= 1;
                    end.unit = this;
                    Path = null;
                    pathIndex = 0;
                    ChangeState(IdleState);
                    if (TargetedUnit != null)
                    {
                        Stats.Energy = 3;
                        readyToAttack = true;
                        ChangeState(attackState);
                    }

                }
                else
                {
                    Stats.Energy -= 1;
                    pathIndex += 1;
                }

            }
        }
    }
}

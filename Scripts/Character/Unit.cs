using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : CharacterBase
{
    private Vector3 velocity = Vector3.forward;
    public string heroName;
    [HideInInspector]
    public int pathIndex = 0;
    public int tileCol;
    public int tileRow;

    [HideInInspector]
    public Sprite heroImage;
    [HideInInspector]
    public Sprite ability1Image;
    [HideInInspector]
    public Sprite ability2Image = null;
    [HideInInspector]
    public Sprite ability3Image = null;


    public int turnOnField = 0;


    public readonly UnitMove moveState = new UnitMove();
    public readonly UnitIdle IdleState = new UnitIdle();
    public readonly UnitAttack attackState = new UnitAttack();
    public readonly UnitAbility abilityState = new UnitAbility();

    public State currentState;
    public int team;
    private Ability ability = null;
    public Ability ability1 = null;
    public Ability ability2 = null;
    public Ability ability3 = null;
    [HideInInspector]
    public bool readyToAttack = false;
    [HideInInspector]
    public HealthBar healthBar;


    public override string AttackType { get => throw new System.NotImplementedException(); protected set => throw new System.NotImplementedException(); }
    public override string Type { get => throw new System.NotImplementedException(); protected set => throw new System.NotImplementedException(); }
    public override int Level { get => throw new System.NotImplementedException(); protected set => throw new System.NotImplementedException(); }
    public override CharacterStats Stats { get => throw new System.NotImplementedException(); protected set => throw new System.NotImplementedException(); }
    public override int UnitsPriority { get; protected set; } = 2;

    public void setUnitPriority(int i)
    {
        this.UnitsPriority = i;
    }

    public int getLevel()
    {
        return Level;
    }
    public Hex[] Path { get;  set; }

    public  Hex Hex { get;  set; }

    public Hex desiredHex { get; set; }
    public Unit TargetedUnit { get; set; }
    public Animator animator { get; set; }
    public AudioSource spellAudio { get; set; }

    public Ability GetAbility()
    {
        return ability;
    }

    public void setAbility(Ability newAbility)
    {
        ability = newAbility;
    }
    private void Update() 
    {

        currentState.Execute(this);
    }

    public void ChangeState(State newState)
    {
        currentState.Exit(this);
        this.currentState = newState;
        currentState.Enter(this);
    }

    public override void Ability()
    {
        throw new System.NotImplementedException();
    }


    public override void Attack(Unit unit)
    {
        if (Stats.Energy > 0)
        {
            unit.Hex.Walkable = true;
            Path = GameManager.Instance.hexMap.return_path(Hex, unit.Hex);
            unit.Hex.Walkable = false;
            if (Stats.AttackRange < Path.Length - 1)
            {
                if (Stats.Energy + Stats.AttackRange > Path.Length - 1)
                {
                    GameManager.Instance.hexMap.ResetGrid();
                    GameManager.Instance.hexMap.clearIndicators();
                    desiredHex = Path[Path.Length - 1 - Stats.AttackRange];
                    Path = null;
                    ChangeState(moveState);
                }
                else
                {
                    Path = null;
                    TargetedUnit = null;
                    ChangeState(IdleState);
                }
            }
            else
            {


                    this.gameObject.transform.LookAt(unit.gameObject.transform);
                    GameManager.Instance.hexMap.ResetGrid();
                    GameManager.Instance.hexMap.clearIndicators();
                    Stats.Energy -= 1;
                    if (GameManager.Instance.player1.turn || GameManager.Instance.player2.turn)
                    {
                    healthBar.showEnergy(Stats.Energy);
                    }
                   
                    this.animator.Play("Attack");

                    if (unit.Stats.Health <= 0)
                    {
                        Destroy(unit.gameObject);
                    }
                    readyToAttack = false;
                    Path = null;
                
            }
        }
        else
        {
            ChangeState(IdleState);
        }
       
       
    }

   

    public override void IncreaseLevel()
    {
        throw new System.NotImplementedException();
    }

   

    protected override float CalculateDamage(float damage)
    {

        return (float)(damage * (1 - ((0.052 * this.Stats.Armor) / (0.9 + 0.048 * Mathf.Abs(this.Stats.Armor))))); 
    }

    public override void RecieveDmg(float damage)
    {

        this.Stats.Health -= CalculateDamage(damage);
        healthBar.SetHealth(this.Stats.Health);
        if (this.Stats.Health <= 0)
        {
            if (team == GameManager.Instance.player1.team)
            {
                GameManager.Instance.player1.squad.Remove(this);
                if (this.UnitsPriority == 1)
                {

                    GameManager.Instance.whoWin.text = "Team 2 WIN";
                    GameManager.Instance.ChangeState(GameManager.Instance.endGame);
                }
            }
            else
            {
                GameManager.Instance.player2.squad.Remove(this);
                if (this.UnitsPriority == 1)
                {
                    GameManager.Instance.whoWin.text = "Team 1 WIN";
                    GameManager.Instance.ChangeState(GameManager.Instance.endGame);
                }
            }
            Destroy(gameObject);
        }
    }
    public override void RecieveMagicDmg(float damage)
    {
        this.Stats.Health -= CalculateMagicDamage(damage);
        healthBar.SetHealth(this.Stats.Health);
        if (this.Stats.Health <= 0)
        {
            if (team == GameManager.Instance.player1.team)
            {
                GameManager.Instance.player1.squad.Remove(this);
                if (this.UnitsPriority == 1)
                {
                    GameManager.Instance.whoWin.text = "Team 2 WIN";
                    GameManager.Instance.ChangeState(GameManager.Instance.endGame);
                }

            }
            else
            {
                GameManager.Instance.player2.squad.Remove(this);
                if (this.UnitsPriority == 1)
                {
                    GameManager.Instance.whoWin.text = "Team 1 WIN";
                    GameManager.Instance.ChangeState(GameManager.Instance.endGame);
                }
            }
            
            Destroy(gameObject);
        }
    }
    protected override float CalculateMagicDamage(float damage)
    {
        return damage * (1 - (this.Stats.MagicResist / 100));
    }



    public override void MoveUnit(Hex end)
    {
        if (Path == null)
        {

            Path = PathFinder.FindPath_AStar(GameManager.Instance.hexMap, Hex, end).ToArray();
            if(Path.Length - 1 > this.Stats.Energy)
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
        
        if(Path != null)
        {
        
        if (pathIndex < Path.Length)
        {

            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(GameManager.Instance.hexMap.positionHex(Path[pathIndex]) - this.transform.position), 300 * Time.deltaTime);
            this.transform.position = Vector3.SmoothDamp(this.transform.position, GameManager.Instance.hexMap.positionHex(Path[pathIndex]), ref velocity, 0.2f);
            

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
                    if (GameManager.Instance.player1.turn || GameManager.Instance.player2.turn)
                    {
                        healthBar.showEnergy(Stats.Energy);
                    }


                    end.unit = this;
                Path = null;
                pathIndex = 0;
                ChangeState(IdleState);
                if (TargetedUnit != null)
                {
                   ChangeState(attackState);
                }

            }
            else
            {
                    Stats.Energy -= 1;
                    if (GameManager.Instance.player1.turn || GameManager.Instance.player2.turn)
                    {
                        healthBar.showEnergy(Stats.Energy);
                    }

                    pathIndex += 1;
            }

        }
    }


    }

   
    public virtual void setEndOfAttack()
    {
    }
   
}

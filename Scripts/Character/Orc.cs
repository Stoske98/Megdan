using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Unit
{

    [HideInInspector]
    public List<Unit> a1InRangeUnits;

    private int turnWhenA1isUsed = 0;
    private void Awake()
    {

        a1InRangeUnits = new List<Unit>();
        ability1 = new Bersek(this);

        heroImage = Resources.Load<Sprite>("Orc/Orc");
        ability1Image = Resources.Load<Sprite>("Orc/Ability1");
    }
    private void Start()
    {
        spellAudio = gameObject.GetComponent<AudioSource>();
        animator = gameObject.GetComponent<Animator>();
        this.Hex = GameManager.Instance.hexMap.GetHex(tileRow, tileCol);
        this.transform.parent = GameObject.Find("HexMap").transform;
        this.transform.localPosition = Hex.getGO().transform.position + new Vector3(0, 0.18f, 0);
        this.Hex.Walkable = false;
        currentState = IdleState;


        GameObject hBar = Resources.Load<GameObject>("Canvas");
        GameObject unitHBar = GameObject.Instantiate(hBar);
        unitHBar.transform.SetParent(this.gameObject.transform);
        unitHBar.transform.localPosition = new Vector3(0, 1.3f, 0);
        healthBar = gameObject.GetComponentInChildren<HealthBar>();
        healthBar.setMaxHealth(Stats.MaxHealth);


    }

    public override int Level { get; protected set; } = 1;
    public override CharacterStats Stats { get; protected set; } = new CharacterStats
    {
        MaxHealth = 1050,
        Health = 1050,

        Damage = 110,

        Energy = 3,
        AttackRange = 1,

        Armor = 10,
        MagicResist = 30


    };

    public override string AttackType { get; protected set; } = "Melee";
    public override string Type { get; protected set; } = "Strenght";

    public override void Ability()
    {

        this.animator.SetBool("Ability", true);
        Stats.Energy -= 1;
        healthBar.showEnergy(Stats.Energy);
        this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);

        GameManager.Instance.updateUnitStats(this);

        turnWhenA1isUsed = GameManager.Instance.numberOfMoves + 1;
        spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
        spellAudio.Play();
    }

    public void checkBersekEnd()
    {
        if (this.ability1.isUsed)
        {
            if (turnWhenA1isUsed == GameManager.Instance.numberOfMoves)
            {
                if (a1InRangeUnits != null)
                {
                    foreach (var unit in a1InRangeUnits)
                    {
                        if (unit.team == this.team)
                        {
                            unit.Stats.Damage /= this.ability1.Quantity;

                        }
                        if (unit.team != this.team)
                        {
                            unit.Stats.Armor += ability1.abilityLvl;
                        }
                    }
                    this.a1InRangeUnits.Clear();
                }
               
                this.ability1.isUsed = false;
                CancelInvoke("checkBersekEnd");
            }
        }
    }

    public override void IncreaseLevel()
    {
        this.Level++;
        CharacterStats newStats;
        float healthPercent = Stats.Health / Stats.MaxHealth;
        switch (this.Level)
        {
            case 2:
                ability1.IncreaseLevel();
                newStats = new CharacterStats
                {
                    MaxHealth = 2100,
                    Health = 2100 * healthPercent,

                    Damage = 220,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 10,
                    MagicResist = 30
                };
                Stats = newStats;
                healthBar.setMaxHealth(Stats.MaxHealth);
                healthBar.SetHealth(Stats.Health);
                GameManager.Instance.updateUnitStats(this);
                break;

            case 3:
                ability1.IncreaseLevel();
                newStats = new CharacterStats
                {
                    MaxHealth = 4200,
                    Health = 4200 * healthPercent,

                    Damage = 440,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 10,
                    MagicResist = 30
                };
                Stats = newStats;
                healthBar.setMaxHealth(Stats.MaxHealth);
                healthBar.SetHealth(Stats.Health);
                GameManager.Instance.updateUnitStats(this);
                break;

        }

    }

    public override void setEndOfAttack()
    {

        this.animator.SetBool("Attack", false);
        this.TargetedUnit.RecieveDmg(this.Stats.Damage);
        GameManager.Instance.updateUnitStats(TargetedUnit);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }
    public void endOfAnimation()
    {

        this.animator.SetBool("Ability", false);
        InvokeRepeating("checkBersekEnd", 1f, 1f);
        GetAbility().useAbility();
        GetAbility().isUsed = true;
        
        ChangeState(IdleState);

    }
}

public class Bersek : Ability
{
    public const string name = "Bersek";
    public const int cooldown = 2;
    public const string description = "Default";
    public const string abilityType = "Physical";
    public const bool istargatable = false;
    public const int range = 1;
    public const float quantity = 2;
    public float armorReduction = 2;
    public Orc orc;

    public Bersek(Orc unit) : base(name, quantity, cooldown, description, abilityType, range)
    {
        this.orc = unit;
    }

    public override void useAbility()
    {

        if (orc != null && orc.Hex != null)
        {
            foreach (var neighbor in PathFinder.BFS_ListInRange(GameManager.Instance.hexMap, orc.Hex, this.Range))
            {
                if (neighbor.unit != null && neighbor.unit.team == orc.team)
                {

                    neighbor.unit.Stats.Damage *= this.Quantity;
                    GameManager.Instance.updateUnitStats(neighbor.unit);
                    orc.a1InRangeUnits.Add(neighbor.unit);
                }
                if (neighbor.unit != null && neighbor.unit.team != orc.team)
                {

                    neighbor.unit.Stats.Armor -= armorReduction;
                    GameManager.Instance.updateUnitStats(neighbor.unit);
                    orc.a1InRangeUnits.Add(neighbor.unit);
                }

                
            }
            orc.Stats.Damage *= this.Quantity;
            GameManager.Instance.updateUnitStats(orc);
            orc.a1InRangeUnits.Add(orc);


        }
    }

    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                armorReduction = 3;
                this.Range = 2;
                break;
            case 3:
                armorReduction = 4;
                this.Range = 2;
                break;

            default:
                break;
        }
    }

}


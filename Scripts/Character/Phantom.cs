using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Phantom : Unit
{

    [Header("Ability1")]
    public ParticleSystem particle1;
    private void Awake()
    {
        ability1 = new RotateAttack(this);
        particle1.Stop();

        heroImage = Resources.Load<Sprite>("Phantom/Phantom");
        ability1Image = Resources.Load<Sprite>("Phantom/Ability1");
    }
    private void Start()
    {
        spellAudio = gameObject.GetComponent<AudioSource>();
        animator = gameObject.GetComponent<Animator>();
        this.Hex = GameManager.Instance.hexMap.GetHex(tileRow, tileCol);
        this.transform.parent = GameObject.Find("HexMap").transform;
        this.transform.localPosition = Hex.getGO().transform.position + new Vector3(0, 0.18f, 0);
        this.Hex.Walkable = false;
        this.Hex.unit = this;
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
        MaxHealth = 750,
        Health = 750,

        Damage = 100,

        Energy = 3,
        AttackRange = 1,

        Armor = 5,
        MagicResist = 25


    };


    public override string AttackType { get; protected set; } = "Melee";
    public override string Type { get; protected set; } = "Agility";


    public override void Ability()
    {
        particle1.Play();
        this.animator.SetBool("Ability", true);
        Stats.Energy -= 1;
        healthBar.showEnergy(Stats.Energy);
        this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
        GetAbility().useAbility();
        GetAbility().isUsed = true;
        spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
        spellAudio.Play();
        GameManager.Instance.updateUnitStats(this);
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
                    MaxHealth = 1500,
                    Health = 1500 * healthPercent,

                    Damage = 200,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 5,
                    MagicResist = 25
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
                    MaxHealth = 3000,
                    Health = 3000 * healthPercent,

                    Damage = 400,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 5,
                    MagicResist = 25
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
        particle1.Stop();
        this.animator.SetBool("Ability", false);
        GetAbility().isUsed = false;
        ChangeState(IdleState);
        
    }
}

public class RotateAttack : Ability
{
    public const string name = "RotateAttack";
    public const int cooldown = 1;
    public const string description = "Default";
    public const string abilityType = "Physical";
    public const bool istargatable = false;
    public Unit heroUnit;
    public const float quantity = 300;



    public RotateAttack( Unit unit) : base(name, quantity, cooldown,description,abilityType,istargatable)
    {
        this.heroUnit = unit;
    }

    public override void useAbility()
    {
       
        if (heroUnit != null && heroUnit.Hex != null )
        {
           
            foreach (var neighbor in GameManager.Instance.hexMap.GetNeighbors(heroUnit.Hex))
            {
               
                if (neighbor.unit != null && neighbor.unit.team != heroUnit.team)
                {
                    neighbor.unit.RecieveDmg(this.Quantity);
                    GameManager.Instance.updateUnitStats(neighbor.unit);
                }
            }

        }
    }
        
      

    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Quantity = 400;
                break;
            case 3:
                this.Quantity = 500;
                break;

            default:
                break;
        }
    }

   
}





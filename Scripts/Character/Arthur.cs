using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arthur : Unit
{
    [Header("Ability1")]
    public GameObject particle1;
    [Header("Ability3")]
    public GameObject particle3;

    private List<Unit> unitsPassive;
    private List<Unit> omni;
    public AudioSource ulty;

    private int turnWhenA3isUsed = 0;
    private float armorbust = 0;

     

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
        unitsPassive = new List<Unit>();
        omni = new List<Unit>();
        ability1 = new Heal(particle1);
        ability2 = new GuardianAngel(2);
        ability3 = new HolyLight(10);


        setUnitPriority(1);

       heroImage = Resources.Load<Sprite>("Arthur/Arthur");
       ability1Image = Resources.Load<Sprite>("Arthur/Ability1");
       ability2Image = Resources.Load<Sprite>("Arthur/Ability2");
       ability3Image = Resources.Load<Sprite>("Arthur/Ability3");

        GameObject hBar = Resources.Load<GameObject>("Canvas");
        GameObject unitHBar = GameObject.Instantiate(hBar);
        unitHBar.transform.SetParent(this.gameObject.transform);
        unitHBar.transform.localPosition = new Vector3(0,1.3f,0);
        healthBar = gameObject.GetComponentInChildren<HealthBar>();
        healthBar.setMaxHealth(Stats.MaxHealth);

        if (team == 2)
        {
            healthBar.showEnergy(0);
        }


    }

    public void checkHolyLightEnd()
    {
        if (ability3.isUsed)
        {
            if (turnWhenA3isUsed == GameManager.Instance.numberOfMoves)
            {
                foreach (var unit in omni)
                {
                    if (unit != null)
                    {
                        unit.Stats.Armor -= armorbust;
                    }

                }
                turnWhenA3isUsed = 0;
                armorbust = 0;
                ability3.isUsed = false;
                CancelInvoke("checkHolyLightEnd");
            }

        }
    }

    public override int Level { get; protected set; } = 1;
    public override CharacterStats Stats { get; protected set; } = new CharacterStats
    {
        MaxHealth = 850,
        Health = 850,

        Damage = 70,

        Energy = 3,
        AttackRange = 1,

        Armor = 7,
        MagicResist = 25


    };


    public override string AttackType { get; protected set; } = "Melee";
    public override string Type { get; protected set; } = "Strenght";


    public override void Ability()
    {
        if (ability1 == this.GetAbility())
        {
            if (TargetedUnit != null)
            {
                if (TargetedUnit.team == team && PathFinder.InRange(GameManager.Instance.hexMap, this.Hex, TargetedUnit.Hex, this.GetAbility().Range))
                {
                    Stats.Energy -= 1;
                    healthBar.showEnergy(Stats.Energy);
                    this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
                    this.animator.SetBool("Ability", true);
                    
                    this.gameObject.transform.LookAt(TargetedUnit.gameObject.transform);
                    GameManager.Instance.updateUnitStats(this);
                }
                else
                {
                    this.ChangeState(IdleState);
                    this.TargetedUnit = null;
                    ability1.isUsed = false;
                }


            }
            else
            {
                this.ChangeState(IdleState);
                ability1.isUsed = false;
            }
        }
        if (ability3 == this.GetAbility())
        {
            Stats.Energy -= 1;
            healthBar.showEnergy(Stats.Energy);
            this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
            this.animator.SetBool("Ability3", true);
            GameManager.Instance.updateUnitStats(this);
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
                ability2.IncreaseLevel();
                ability3.IncreaseLevel();
                newStats = new CharacterStats
                {
                    MaxHealth = 1700,
                    Health = 1700 * healthPercent,

                    Damage = 140,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 7,
                    MagicResist = 25
                };
                Stats = newStats;
                healthBar.setMaxHealth(Stats.MaxHealth);
                healthBar.SetHealth(Stats.Health);
                GameManager.Instance.updateUnitStats(this);
                break;

            case 3:
                ability1.IncreaseLevel();
                ability2.IncreaseLevel();
                ability3.IncreaseLevel();
                newStats = new CharacterStats
                {
                    MaxHealth = 3400,
                    Health = 3400 * healthPercent,

                    Damage = 360,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 7,
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
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }

    public void dealHalfDmg()
    {
        this.TargetedUnit.RecieveDmg(this.Stats.Damage / 2);
        GameManager.Instance.updateUnitStats(TargetedUnit);
    }
    public void HolyLight()
    {


        if (this.team == GameManager.Instance.player1.team)
        {
            foreach (var unit1 in GameManager.Instance.player1.squad)
            {
                unit1.Stats.Armor += GetAbility().Quantity;
                GameManager.Instance.updateUnitStats(unit1);
                GameObject hl = GameObject.Instantiate(particle3, unit1.transform.position, Quaternion.identity);
                GameObject.Destroy(hl, 3);
                omni.Add(unit1);
            }
        }
        if (this.team == GameManager.Instance.player2.team)
        {
            foreach (var unit2 in GameManager.Instance.player2.squad)
            {
                unit2.Stats.Armor += GetAbility().Quantity;
                GameManager.Instance.updateUnitStats(unit2);
                GameObject hl = GameObject.Instantiate(particle3, unit2.transform.position, Quaternion.identity);
                GameObject.Destroy(hl, 3);
                omni.Add(unit2);
            }
        }
        armorbust = GetAbility().Quantity;
        turnWhenA3isUsed = GameManager.Instance.numberOfMoves + 2;
        ulty.volume = SettingsControll.Instance.audioSliderEff.value;
        ulty.Play();

        this.animator.SetBool("Ability3", false);
        this.TargetedUnit = null;
        ChangeState(IdleState);

        InvokeRepeating("checkHolyLightEnd", 1f, 1f);
    }

    public void Heal()
    {

        spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
        spellAudio.Play();
        ability1.useAbility(TargetedUnit);
        ability1.isUsed = false;
        this.animator.SetBool("Ability", false);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }

    public void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();
        if (unit != null && unit.team == team)
        {
            
            unitsPassive.Add(unit);
            unit.Stats.Armor += this.ability2.Quantity;
            GameManager.Instance.updateUnitStats(unit);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();
        if (unit != null && unit.team == team)
        {
            unitsPassive.Remove(unit);
            unit.Stats.Armor -= this.ability2.Quantity;
            GameManager.Instance.updateUnitStats(unit);
        }
    }
}

public class GuardianAngel : Ability
{
    public const string name = "GuardianAngel";

    public const string description = "GuardianAngel";
    public const string abilityType = "Passive";
    public GuardianAngel(float quantity)
    {
        this.isPassive = true;
        this.Quantity = quantity;
    }
    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Quantity = 3;
                break;
            case 3:
                this.Quantity = 4;
                break;

            default:
                break;
        }
    }
}

public class HolyLight : Ability
{

    public const string name = "HolyLight";
    public const int cooldown = 2;

    public const string description = "Default";
    public const string abilityType = "Active";
    public HolyLight(float quantity)
    {
        this.Quantity = quantity;
        this.maxCoolDown = cooldown;
    }
    
    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Quantity = 50;
                break;
            case 3:
                this.Quantity = 100;
                break;

            default:
                break;
        }
    }
}




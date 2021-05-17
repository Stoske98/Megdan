using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antimage : Unit
{
    [Header("Ability1")]
    public GameObject particle1;

    private int turnWhenA1isUsed = 0;
    private float dmgbust = 0;
    private void Awake()
    {

        ability1 = new Blink(this.gameObject.transform, particle1);
        heroImage = Resources.Load<Sprite>("Antimage/Antimage");
        ability1Image = Resources.Load<Sprite>("Antimage/Ability1");
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
    public void checkCritEnd()
    {
        if (this.ability1.isUsed)
        {
            if (turnWhenA1isUsed == GameManager.Instance.numberOfMoves)
            {
                this.Stats.Damage /= dmgbust;
                dmgbust = 0;
                this.ability1.isUsed = false;
                CancelInvoke("checkCritEnd");
            }
        }
       
    }

    public override int Level { get; protected set; } = 1;
    public override CharacterStats Stats { get; protected set; } = new CharacterStats
    {
        MaxHealth = 550,
        Health = 550,

        Damage = 80,

        Energy = 3,
        AttackRange = 1,

        Armor = 5,
        MagicResist = 30


    };


    public override string AttackType { get; protected set; } = "Melee";
    public override string Type { get; protected set; } = "Agility";


    public override void Ability()
    {

        if (ability1 == this.GetAbility())
        {
            if (desiredHex != null)
            {
                if ( PathFinder.InRange(GameManager.Instance.hexMap, this.Hex, desiredHex, this.GetAbility().Range))
                {
                    
                    Stats.Energy -= 1;
                    healthBar.showEnergy(Stats.Energy);
                    this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
                    this.Hex.Walkable = true;
                    this.Hex = desiredHex;
                    this.Hex.unit = this;
                    this.gameObject.transform.LookAt(GameManager.Instance.hexMap.positionHex(desiredHex));
                    this.animator.SetBool("Ability", true);
                    this.GetAbility().isUsed = true;
                    this.Stats.Damage *= this.GetAbility().Quantity;
                    GameManager.Instance.updateUnitStats(this);
                    dmgbust = this.GetAbility().Quantity;
                    turnWhenA1isUsed = GameManager.Instance.numberOfMoves + 1;

                }
                else
                {
                    this.ChangeState(IdleState);
                    this.desiredHex = null;
                    ability1.isUsed = false;
                }
            }
            else
            {
                this.ChangeState(IdleState);
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
                    MaxHealth = 1100,
                    Health = 1100 * healthPercent,

                    Damage = 160,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 5,
                    MagicResist = 40
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
                    MaxHealth = 2200,
                    Health = 2200 * healthPercent,

                    Damage = 320,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 5,
                    MagicResist = 50
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

    public void Blink()
    {
        GetAbility().useAbility(desiredHex);
        desiredHex.Walkable = false;
        this.animator.SetBool("Ability", false);
        this.desiredHex = null;
        spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
        spellAudio.Play();
        InvokeRepeating("checkCritEnd", 1f, 1f);
        ChangeState(IdleState);
    }


}

public class Blink : Ability
{
    public GameObject Particle;

    public const string name = "Blink";
    public const int cooldown = 2;

    public const string description = "Default";
    public const string abilityType = "Physical";
    public const bool istargatable = true;
    public Transform amTransfrom;
    public const float quantity = 2;
    public const int range = 2;

    public Blink(Transform am, GameObject particle) : base(name, quantity, cooldown, description, abilityType, istargatable, range)
    {
        this.HexIsTargetable = true;
        this.amTransfrom = am;
        this.Particle = particle;

    }
    public override void useAbility(Hex hex)
    {
        GameObject heal = GameObject.Instantiate(Particle, amTransfrom.position, Quaternion.identity);
        GameObject.Destroy(heal,3);
        amTransfrom.position = GameManager.Instance.hexMap.positionHex(hex);

    }


    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Range = 3;
                this.Quantity = 2.5f;
                break;
            case 3:
                this.Range = 4;
                this.Quantity = 3f;
                break;

            default:
                break;
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ukora : Unit
{
    [Header("Ability1")]
    public ParticleSystem particle1;
    public GameObject possessParticle;

    [Header("NormalAttack")]
    public Transform attackPosition;
    public GameObject attackGO;

    private int turnWhenA1isUsed = 0;
    private Unit possessUnit;
    private void Awake()
    {

        ability1 = new Possess(particle1);

        particle1.Stop();

        heroImage = Resources.Load<Sprite>("Ukora/Ukora");
        ability1Image = Resources.Load<Sprite>("Ukora/Ability1");
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
        MaxHealth = 450,
        Health = 450,

        Damage = 50,

        Energy = 3,
        AttackRange = 3,

        Armor = 5,
        MagicResist = 10


    };
    public override string AttackType { get; protected set; } = "Range";
    public override string Type { get; protected set; } = "Intelligence";

    public override void Ability()
    {
        if (TargetedUnit != null)
        {
            if (TargetedUnit.team != team && PathFinder.InRange(GameManager.Instance.hexMap, this.Hex, TargetedUnit.Hex, this.GetAbility().Range))
            {
                Stats.Energy -= 1;
                healthBar.showEnergy(Stats.Energy);
                this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
                this.animator.SetBool("Ability", true);
                this.gameObject.transform.LookAt(TargetedUnit.gameObject.transform);
                turnWhenA1isUsed = GameManager.Instance.numberOfMoves + 1;

                GameManager.Instance.updateUnitStats(this);
            }
            else
            {
                this.ChangeState(IdleState);
                this.TargetedUnit = null;
                this.ability1.isUsed = false;
            }


        }
        else
        {
            this.ChangeState(IdleState);
        }

    }
    public void checkPossessEnd()
    {
        if (this.ability1.isUsed)
        {
            if (turnWhenA1isUsed == GameManager.Instance.numberOfMoves)
            {
                if (possessUnit.team == 1)
                {
                    possessUnit.team = 2;
                }
                else if(possessUnit.team == 2)
                {
                    possessUnit.team = 1;
                }

                if (possessUnit.ability1 != null)
                {
                    possessUnit.ability1.setPassive(false);
                }
                if (possessUnit.ability2 != null)
                {
                    possessUnit.ability2.setPassive(false);
                }
                if (possessUnit.ability3 != null)
                {
                    possessUnit.ability3.setPassive(false);
                }
                Destroy(GameObject.FindWithTag("Possess"));
                this.ability1.isUsed = false;
                CancelInvoke("checkPossessEnd");
            }
        }
    }
    public void Possess()
    {
        GetAbility().useAbility(this);
        Invoke("InstantiateParticle", 0.3f);
        spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
        spellAudio.Play();
        this.animator.SetBool("Ability", false);
        InvokeRepeating("checkPossessEnd", 1f, 1f);
        ChangeState(IdleState);
    }

    public void InstantiateParticle()
    {
        GameObject go = GameObject.Instantiate(possessParticle, this.TargetedUnit.gameObject.transform.position + new Vector3(0, 0.6f, 0), Quaternion.identity);
        go.transform.parent = this.TargetedUnit.gameObject.transform;
        possessUnit = this.TargetedUnit;
        this.TargetedUnit = null;
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
                    MaxHealth = 900,
                    Health = 900 * healthPercent,

                    Damage = 90,

                    Energy = 3,
                    AttackRange = 3,

                    Armor = 5,
                    MagicResist = 10
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
                    MaxHealth = 1800,
                    Health = 1800 * healthPercent,

                    Damage = 180,

                    Energy = 3,
                    AttackRange = 3,

                    Armor = 5,
                    MagicResist = 10
                };
                Stats = newStats;
                healthBar.setMaxHealth(Stats.MaxHealth);
                healthBar.SetHealth(Stats.Health);
                GameManager.Instance.updateUnitStats(this);
                break;

        }

    }

    public void normalAttack()
    {
        GameObject missileGO = GameObject.Instantiate(attackGO, attackPosition.position, Quaternion.identity);
        Missile missile = missileGO.GetComponent<Missile>();
        missile.moveSpeed = 5;
        missile.enemy = TargetedUnit;
        missile.DamageToDeal = this.Stats.Damage;


    }

    public override void setEndOfAttack()
    {

        this.animator.SetBool("Attack", false);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }
}

public class Possess : Ability
{
    public ParticleSystem Particle;

    public const string name = "Possess";
    public const int cooldown = 1;

    public const string description = "Default";
    public const string abilityType = "Magical";
    public const bool istargatable = true;
    public const float quantity = 0;
    public const int range = 1;

    public Possess(ParticleSystem particle) : base(name, quantity, cooldown, description, abilityType, istargatable, range)
    {
        this.Particle = particle;
    }
    
    public override void useAbility(Unit unit)
    {

        Particle.Play();
        
        unit.TargetedUnit.team = unit.team;
        unit.TargetedUnit.Stats.Energy = 3;

        if (unit.TargetedUnit.ability1 != null)
        {
            unit.TargetedUnit.ability1.setPassive(true); 
        }
        if (unit.TargetedUnit.ability2 != null)
        {
            unit.TargetedUnit.ability2.setPassive(true);
        }
        if (unit.TargetedUnit.ability3 != null)
        {
            unit.TargetedUnit.ability3.setPassive(true);
        }
    }
    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Range = 2;
                break;
            case 3:
                this.Range = 3;
                break;

            default:
                break;
        }
    }

}


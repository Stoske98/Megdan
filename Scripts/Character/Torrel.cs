using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Torrel : Unit 
{
    public GameObject neutral;
    private List<Unit> neutrals;

    private Hex spawnPos;

    private bool herospawned = false;
    private void Awake()
    {
        neutrals = new List<Unit>();
        ability1 = new SpawnFiend();
        heroImage = Resources.Load<Sprite>("Torrel/Torrel");
        ability1Image = Resources.Load<Sprite>("Torrel/Ability1");
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
        MaxHealth = 800,
        Health = 800,

        Damage = 70,

        Energy = 3,
        AttackRange = 1,

        Armor = 5,
        MagicResist = 25


    };


    public override string AttackType { get; protected set; } = "Melee";
    public override string Type { get; protected set; } = "Strenght";


    public override void Ability()
    {
        
        foreach (var neighbor in GameManager.Instance.hexMap.GetNeighbors(this.Hex))
        {

            if (neighbor.Walkable)
            {
                spawnPos = neighbor;
                herospawned = true;
                this.animator.SetBool("Ability", true);
                spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
                spellAudio.Play();

                Stats.Energy -= 1;
                healthBar.showEnergy(Stats.Energy);
                this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);

                GameManager.Instance.updateUnitStats(this);

                ability1.isUsed = true;

                break;
            }
        }

        if (!herospawned)
        {
            this.ChangeState(IdleState);
            this.TargetedUnit = null;
            ability1.isUsed = false;

        }
        else
        {
            herospawned = false;
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
                    MaxHealth = 1600,
                    Health = 1600 * healthPercent,

                    Damage = 140,

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
                    MaxHealth = 3200,
                    Health = 3200 * healthPercent,

                    Damage = 280,

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

    public void Spawn()
    {
        GameObject unitGo = GameObject.Instantiate(neutral, new Vector3(0, 0, 0), Quaternion.identity);
        Unit n = unitGo.GetComponent<Unit>();
        if (n != null)
        {
            neutrals.Add(n);
            n.tileCol = spawnPos.Col;
            n.tileRow = spawnPos.Row;
            n.team = this.team;

            if (this.Level == 2)
            {
                n.gameObject.transform.localScale = new Vector3(1.25f,1.25f,1.25f);
                n.IncreaseLevel();
            }
            if (this.Level == 3)
            {
                n.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                n.IncreaseLevel();
                n.IncreaseLevel();

            }

        }

        if (GameManager.Instance.player1.team == team)
        {
            GameManager.Instance.player1.squad.Add(n);
        }
        else
        {
            GameManager.Instance.player2.squad.Add(n);
        }

        ability1.isUsed = false;
        this.animator.SetBool("Ability", false);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }

    

    public override void setEndOfAttack()
    {

        this.animator.SetBool("Attack", false);
        this.TargetedUnit.RecieveDmg(this.Stats.Damage);
        GameManager.Instance.updateUnitStats(TargetedUnit);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }

   

  

    
}

public class SpawnFiend : Ability
{
    public ParticleSystem Particle;
    public GameObject neutral;
    public List<Unit> spawnedUnits;

    public const string name = "Fiend";
    public const int cooldown = 2;

    public const string description = "Default";
    public const string abilityType = "Magical";
    public const bool istargatable = false;
    public const int range = 1;

    public SpawnFiend() : base(name, cooldown, description, abilityType, range)
    {
    }

    public override void useAbility()
    {

    }



}


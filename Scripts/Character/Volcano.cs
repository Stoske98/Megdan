
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Volcano : Unit 
{
    [Header("Ability1")]
    public GameObject particlea11;
    public GameObject particlea12;

    [HideInInspector]
    public List<Unit> a1InRangeUnits;
    [HideInInspector]
    public List<GameObject> a1stuns;

    private int turnWhenA1isUsed = 0;
    private void Awake()
    {

        ability1 = new Eartshaker(this, particlea11, particlea12);

        heroImage = Resources.Load<Sprite>("Volcano/Volcano");
        ability1Image = Resources.Load<Sprite>("Volcano/Ability1");
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
        MaxHealth = 1000,
        Health = 1000,

        Damage = 90,

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

        turnWhenA1isUsed = GameManager.Instance.numberOfMoves + 2;
    }

    public void checkErtshakerEnd()
    {
        if (this.ability1.isUsed)
        {
            if (turnWhenA1isUsed == GameManager.Instance.numberOfMoves)
            {
                if (a1InRangeUnits != null)
                {
                    foreach (var unit in a1InRangeUnits)
                    {
                        if (this.team == 1)
                        {
                            unit.team = 2;
                        }
                        else if (this.team == 2)
                        {
                            unit.team = 1;
                        }

                    }
                    this.a1InRangeUnits.Clear();
                }
                if (a1stuns != null)
                {
                    foreach (var stun in a1stuns)
                    {
                        Destroy(stun);
                    }
                    a1stuns.Clear();
                }
                
                this.ability1.isUsed = false;
                CancelInvoke("checkErtshakerEnd");
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
                    MaxHealth = 2000,
                    Health = 2000 * healthPercent,

                    Damage = 180,

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
                    MaxHealth = 4000,
                    Health = 4000 * healthPercent,

                    Damage = 360,

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
        InvokeRepeating("checkErtshakerEnd", 1f, 1f);
        GetAbility().useAbility();
        spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
        spellAudio.Play();
        GetAbility().isUsed = true;

        ChangeState(IdleState);

    }
}

public class Eartshaker : Ability
{

    public const string name = "RotateAttack";
    public const int cooldown = 2;
    public const string description = "Default";
    public const string abilityType = "Physical";
    public const bool istargatable = false;
    private Volcano volcano;
    private GameObject stunParticle;
    private GameObject eartShaker;
    public const float quantity = 150;
    public const int range = 1;

    public Eartshaker(Volcano unit, GameObject stunParticle, GameObject eartshakerParticle) : base(name, quantity, cooldown, description, abilityType, range)
    {
        this.eartShaker = eartshakerParticle;
        this.stunParticle = stunParticle;
        this.volcano = unit;
    }

    public override void useAbility()
    {
        Vector3 v = new Vector3(0,0.2f,0);
        if (volcano != null && volcano.Hex != null)
        {

            foreach (var neighbor in PathFinder.BFS_ListInRange(GameManager.Instance.hexMap, volcano.Hex, this.Range))
            {
                if (neighbor.getGO().activeInHierarchy == true)
                {
                    GameObject es = GameObject.Instantiate(eartShaker, neighbor.getGO().transform.position + v, Quaternion.identity);
                    GameObject.Destroy(es, 2);
                }
                

                if (neighbor.unit != null && neighbor.unit.team != volcano.team && neighbor.getGO().activeInHierarchy == true)
                {

                    GameObject es = GameObject.Instantiate(eartShaker, neighbor.getGO().transform.position + v, Quaternion.identity);
                    GameObject.Destroy(es, 2);
                    volcano.a1stuns.Add(GameObject.Instantiate(stunParticle, neighbor.unit.transform.position, Quaternion.identity));
                    neighbor.unit.RecieveMagicDmg(this.Quantity);
                    GameManager.Instance.updateUnitStats(neighbor.unit);
                    neighbor.unit.team = 0;
                    volcano.a1InRangeUnits.Add(neighbor.unit);
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
                this.Range = 2;
                this.Quantity = 250;
                break;
            case 3:
                this.Range = 3;
                this.Quantity = 350;
                break;

            default:
                break;
        }
    }
}



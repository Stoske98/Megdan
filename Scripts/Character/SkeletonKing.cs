
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKing : Unit
{
    [Header("Ability2")]
    public ParticleSystem particle2;

    public GameObject neutralSkeleton;
    private List<Unit> neutrals;

    Crit crit;
    LifeSteal ls;
    private int turnWhenA1isUsed = 0;
    float dmgToDeal;
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
        neutrals = new List<Unit>();

        ability1 = new SpawnSkeletons(this,neutralSkeleton,neutrals);
        crit = new Crit();
        ability3 = crit;
        ls = new LifeSteal();
        ability2 = ls;

        particle2.Stop();

        setUnitPriority(1);

        heroImage = Resources.Load<Sprite>("SkeletonKing/SkeletonKing");
        ability1Image = Resources.Load<Sprite>("SkeletonKing/Ability1");
        ability2Image = Resources.Load<Sprite>("SkeletonKing/Ability2");
        ability3Image = Resources.Load<Sprite>("SkeletonKing/Ability3");

        GameObject hBar = Resources.Load<GameObject>("Canvas");
        GameObject unitHBar = GameObject.Instantiate(hBar);
        unitHBar.transform.SetParent(this.gameObject.transform);
        unitHBar.transform.localPosition = new Vector3(0, 1.3f, 0);
        healthBar = gameObject.GetComponentInChildren<HealthBar>();
        healthBar.setMaxHealth(Stats.MaxHealth);



        if (team == 2)
        {
            healthBar.showEnergy(0);
        }

    }
    public void checkSkeletonsEnd()
    {
        if (ability1.isUsed)
        {
            if (turnWhenA1isUsed == GameManager.Instance.numberOfMoves)
            {
                if (neutrals != null)
                {
                    foreach (var unit in neutrals)
                    {

                        unit.Hex.Walkable = true;
                        Destroy(unit.gameObject);

                    }
                    neutrals.Clear();
                }
                turnWhenA1isUsed = 0;
                ability1.isUsed = false;
                CancelInvoke("checkSkeletonsEnd");
            }

        }
    }
    public override int Level { get; protected set; } = 1;
    public override CharacterStats Stats { get; protected set; } = new CharacterStats
    {
        MaxHealth = 750,
        Health = 750,

        Damage = 90,

        Energy = 3,
        AttackRange = 1,

        Armor = 5,
        MagicResist = 25


    };

    public override string AttackType { get; protected set; } = "Melee";
    public override string Type { get; protected set; } = "Strenght";

    public override void Ability()
    {
        if (GetAbility() == ability1)
        {
            if (TargetedUnit.team != team && PathFinder.InRange(GameManager.Instance.hexMap, this.Hex, TargetedUnit.Hex, ability1.Range))
            {
                Stats.Energy -= 1;
                healthBar.showEnergy(Stats.Energy);
                this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
                this.animator.SetBool("Ability", true);
                this.gameObject.transform.LookAt(TargetedUnit.gameObject.transform);
                spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
                spellAudio.Play();
                GameManager.Instance.updateUnitStats(this);
            }
            else
            {
                this.ChangeState(IdleState);
                this.TargetedUnit = null;
                this.ability1.isUsed = false;
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
                ability2.IncreaseLevel();
                ability3.IncreaseLevel();
                newStats = new CharacterStats
                {
                    MaxHealth = 1500,
                    Health = 1500 * healthPercent,

                    Damage = 180,

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
                ability2.IncreaseLevel();
                ability3.IncreaseLevel();
                newStats = new CharacterStats
                {
                    MaxHealth = 3000,
                    Health = 3000 * healthPercent,

                    Damage = 360,

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

    public void SpawnSkeletons()
    {
        turnWhenA1isUsed = GameManager.Instance.numberOfMoves + 1;
        ability1.useAbility();
        this.animator.SetBool("Ability", false);
        this.TargetedUnit = null;
        InvokeRepeating("checkSkeletonsEnd", 1f, 1f);
        ChangeState(IdleState);
    }

    public override void setEndOfAttack()
    {
        this.animator.SetBool("Attack", false);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }

    public void normalAttack()
    {
        dmgToDeal = crit.calculateDmg(this.Stats.Damage);
        particle2.Play();

        if (this.Stats.MaxHealth < this.Stats.Health + ls.calculateLifeSteal(dmgToDeal))
        {
            this.Stats.Health = this.Stats.MaxHealth;
            GameManager.Instance.updateUnitStats(this);
        }
        else
        {
            this.Stats.Health += ls.calculateLifeSteal(dmgToDeal);
            GameManager.Instance.updateUnitStats(this);
        }
        this.TargetedUnit.RecieveDmg(dmgToDeal);
        GameManager.Instance.updateUnitStats(TargetedUnit);


    }

}

public class LifeSteal : Ability
{
    public const string name = "LifeSteal";

    public const string description = "Default";
    public const string abilityType = "Passive";

    public LifeSteal()
    {
        this.isPassive = true;
        this.Quantity = 18f;
    }
    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Quantity = 26f;
                break;
            case 3:
                this.Quantity = 38f;
                break;

            default:
                break;
        }
    }

    public float calculateLifeSteal(float dmg)
    {
        return (dmg / 100) * this.Quantity;
    }
}


public class SpawnSkeletons : Ability
{

    public ParticleSystem Particle;
    public GameObject neutral;
    public List<Unit> spawnedUnits;

    public const string name = "Fiend";
    public const int cooldown = 2;

    public const string description = "Default";
    public const string abilityType = "Magical";
    public const bool istargatable = false;
    public const float quantity = 2;
    public const int range = 2;
    public Unit skeletonKing;
    private int calc = 0;

    public SpawnSkeletons(Unit sk, GameObject neutral, List<Unit> spawnedUnits) : base(name,quantity ,cooldown, description, abilityType, range)
    {
        this.isTargetable = true;
        this.skeletonKing = sk;
        this.neutral = neutral;
        this.spawnedUnits = spawnedUnits;
    }

    public override void useAbility()
    {
        foreach (var neighbor in GameManager.Instance.hexMap.GetNeighbors(skeletonKing.Hex))
        {

            if (neighbor.Walkable)
            {
                    foreach (var hexes in GameManager.Instance.hexMap.GetNeighbors(skeletonKing.TargetedUnit.Hex))
                    {
                        if (hexes.Walkable)
                        {
                            GameObject unitGo = GameObject.Instantiate(neutral, new Vector3(0, 0, 0), Quaternion.identity);
                            Unit unit = unitGo.GetComponent<Unit>();
                            if (unit != null)
                            {
                                spawnedUnits.Add(unit);
                                unit.animator = unit.gameObject.GetComponent<Animator>();
                                unit.tileCol = neighbor.Col;
                                unit.tileRow = neighbor.Row;
                                unit.team = 0;
                                unit.Hex = GameManager.Instance.hexMap.GetHex(unit.tileRow, unit.tileCol);
                                unit.transform.parent = GameObject.Find("HexMap").transform;
                                unit.transform.localPosition = unit.Hex.getGO().transform.position + new Vector3(0, 0.18f, 0);
                                unit.Hex.Walkable = false;
                                unit.Hex.unit = unit;
                                unit.currentState = unit.IdleState;
                                if (skeletonKing.getLevel() == 2)
                                {
                                    unit.IncreaseLevel();
                                }
                                else if (skeletonKing.getLevel() == 3)
                                {
                                    unit.IncreaseLevel();
                                    unit.IncreaseLevel();
                                }
                            }
                            unit.TargetedUnit = skeletonKing.TargetedUnit;
                            unit.desiredHex = hexes;
                            hexes.Walkable = false;
                            calc++;
                            break;
                        }
                    }
                
                if (calc == this.Quantity)
                {
                    foreach (var sk in spawnedUnits)
                    {
                        sk.desiredHex.Walkable = true;
                        sk.ChangeState(sk.moveState);
                    }
                    break;
                }
            }
        }
        if (calc < this.Quantity && calc != 0)
        {
            foreach (var sk in spawnedUnits)
            {
                sk.desiredHex.Walkable = true;
                sk.ChangeState(sk.moveState);
            }
        }

        calc = 0;
    }

    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Quantity = 3;
                break;
            case 3:;
                this.Quantity = 4;
                break;

            default:
                break;
        }
    }
}
public class Crit : Ability
{

    public const string name = "Crit";

    public const string description = "Default";
    public const string abilityType = "Passive";

    public float procChance = 0.15f;
    private float f;
    public Crit()
    {
        this.isPassive = true;
        this.Quantity = 2;
    }
    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                procChance = 0.2f;
                this.Quantity = 3.25f;
                break;
            case 3:
                procChance = 0.25f;
                this.Quantity = 4.5f;
                break;

            default:
                break;
        }
    }

    public float calculateDmg(float dmg)
    {
        f = Random.Range(0f, 1f);
        if (f < this.procChance)
        {
            return dmg * this.Quantity;
        }
        return dmg;
    }

}



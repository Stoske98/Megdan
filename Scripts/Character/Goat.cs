using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Goat : Unit 
{

    [Header("NormalAttack")]
    public Transform ArrowPosition;
    public GameObject arrowkGO;

    [Header("Ability1")]
    public GameObject scaledHex;
    public GameObject particle1;

    private Vector3 V = Vector3.forward;

    private int turnWhenA1isUsed = 0;
    private float dmgbust = 0;
    private void Awake()
    {

        ability1 = new InfinitiyRange(this.gameObject.transform, scaledHex, particle1);
        heroImage = Resources.Load<Sprite>("Goat/Goat");
        ability1Image = Resources.Load<Sprite>("Goat/Ability1");
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
                this.Stats.AttackRange -= 10;
                dmgbust = 0;
                this.ability1.isUsed = false;
                Destroy(GameObject.FindWithTag("Tower"));
                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 1.8f, this.gameObject.transform.position.z);
                spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
                 spellAudio.Play();
                CancelInvoke("checkCritEnd");

            }
        }

    }

    public override int Level { get; protected set; } = 1;
    public override CharacterStats Stats { get; protected set; } = new CharacterStats
    {
        MaxHealth = 450,
        Health = 450,

        Damage = 70,

        Energy = 3,
        AttackRange = 3,

        Armor = 5,
        MagicResist = 10


    };
    public override string AttackType { get; protected set; } = "Range";
    public override string Type { get; protected set; } = "Archer";

    public override void Ability()
    {
        GetAbility().useAbility();

        this.Stats.AttackRange += 10;
        dmgbust = this.GetAbility().Quantity;
        this.Stats.Damage *= dmgbust;

        GetAbility().isUsed = true;

        Stats.Energy -= 1;
        healthBar.showEnergy(Stats.Energy);
        this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);

        GameManager.Instance.updateUnitStats(this);

        turnWhenA1isUsed = GameManager.Instance.numberOfMoves + 1;
        InvokeRepeating("checkCritEnd", 1f, 1f);
        spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
        spellAudio.Play();

        ChangeState(IdleState);

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

                    Damage = 140,

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

                    Damage = 280,

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

    public override void setEndOfAttack()
    {

        this.animator.SetBool("Attack", false);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }

    public void normalAttack()
    {
        GameObject missileGO = GameObject.Instantiate(arrowkGO, ArrowPosition.position, Quaternion.identity);
        ParticleSystem ps = missileGO.GetComponentInChildren<ParticleSystem>();

        ps.gameObject.SetActive(false);
        if (ability1.isUsed)
        {
            ps.gameObject.SetActive(true);
            ps.Play();
        }
        Missile missile = missileGO.GetComponent<Missile>();
        missile.enemy = TargetedUnit;
        missile.DamageToDeal = this.Stats.Damage;


    }

    public override void MoveUnit(Hex end)
    {
        if (!ability1.isUsed)
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
        else
        {
            ChangeState(IdleState);
        }
    }
       

}

public class InfinitiyRange : Ability
{

    public GameObject Particle;
    public GameObject ScaledHex;
    public const string name = "Infinity Range";
    public const int cooldown = 2;

    public const string description = "Default";
    public const string abilityType = "Physical";
    public const bool istargatable = false;
    public Transform goatTransform;
    public const float quantity = 2;

    public InfinitiyRange(Transform goat, GameObject scaledHex, GameObject particle) : base(name, quantity, cooldown, description, abilityType, istargatable)
    {
        this.ScaledHex = scaledHex;
        this.goatTransform = goat;
        this.Particle = particle;

    }

    public override void useAbility()
    {

        GameObject.Instantiate(ScaledHex, goatTransform.position - new Vector3(0,0.2f,0), Quaternion.identity);
        goatTransform.position = new Vector3(goatTransform.position.x, goatTransform.position.y + 1.8f, goatTransform.position.z);


    }

    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Quantity = 2.5f;
                break;
            case 3:
                this.Quantity = 3f;
                break;

            default:
                break;
        }
    }

}




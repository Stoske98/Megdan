using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diana : Unit
{
    [Header("Ability1")]

    public Transform spawnPosArrow;
    public GameObject ArrowToSpawn;
    public GameObject arrowParticle;

    [Header("NormalAttack")]
    public Transform ArrowPosition;
    public GameObject arrowkGO;

    private void Awake()
    {

        ability1 = new PowerShot();

        heroImage = Resources.Load<Sprite>("Diana/Diana");
        ability1Image = Resources.Load<Sprite>("Diana/Ability1");
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

        Damage = 135,

        Energy = 3,
        AttackRange = 3,

        Armor = 5,
        MagicResist = 10

    };
    public override string AttackType { get; protected set; } = "Range";
    public override string Type { get; protected set; } = "Archer";

    public override void Ability()
    {
        GameObject arrowAbil = GameObject.Instantiate(ArrowToSpawn, spawnPosArrow.position, spawnPosArrow.rotation);
        ArrowDirection arrow = arrowAbil.GetComponent<ArrowDirection>();
        arrow.DamageToDeal = ability1.Quantity;
        arrow.diana = this;

        arrowParticle.SetActive(false);
        animator.SetBool("Ability", false);
        this.ChangeState(IdleState);
        this.ability1.isUsed = false;

        Stats.Energy -= 1;
        healthBar.showEnergy(Stats.Energy);
        this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
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

                    Damage = 210,

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
                    MaxHealth = 3000,
                    Health = 3000 * healthPercent,

                    Damage = 340,

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
     
        Missile missile = missileGO.GetComponent<Missile>();
        missile.enemy = TargetedUnit;
        missile.DamageToDeal = this.Stats.Damage;


    }

    public void activateParticle()
    {
        arrowParticle.SetActive(true);
    }
}


public class PowerShot : Ability
{
    public Transform spawnPojectilPosition;
    public GameObject Projectil;

    public const string name = "PowerShot";
    public const int cooldown = 2;
    public const string description = "Default";
    public const string abilityType = "Magic";
    public const float quantity = 400;

    public PowerShot() : base(name, cooldown, description, abilityType)
    {
        this.Quantity = quantity;
        this.freeDirection = true;
    }

    
    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Quantity = 600;

                break;
            case 3:
                this.Quantity = 800;

                break;

            default:
                break;
        }
    }

}

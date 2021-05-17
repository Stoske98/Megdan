using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Vallee : Unit
{
    [Header("NormalAttack")]
    public Transform attackPosition;
    public GameObject attackGO;

    [Header("Ability1")]
    public GameObject skull;
    public Transform spawnPos;
    private void Awake()
    {
        heroImage = Resources.Load<Sprite>("Vallee/Vallee");
        ability1Image = Resources.Load<Sprite>("Vallee/Ability1");
        ability1 = new LifeDrain(skull, spawnPos);
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
        MaxHealth = 550,
        Health = 550,

        Damage = 50,

        Energy = 3,
        AttackRange = 3,

        Armor = 5,
        MagicResist = 10


    };


    public override string AttackType { get; protected set; } = "Range";
    public override string Type { get; protected set; } = "Druid";


    public override void Ability()
    {
        if (TargetedUnit.team == team && PathFinder.InRange(GameManager.Instance.hexMap, this.Hex, TargetedUnit.Hex, this.GetAbility().Range))
        {
            if (TargetedUnit.Stats.Energy < 3)
            {
                Stats.Energy -= 1;
                healthBar.showEnergy(Stats.Energy);
                this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
                this.gameObject.transform.LookAt(TargetedUnit.gameObject.transform);
                this.animator.SetBool("Ability", true);
                spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
                spellAudio.Play();
                GameManager.Instance.updateUnitStats(TargetedUnit);

            }
            else
            {

                this.ChangeState(IdleState);
                this.TargetedUnit = null;
                this.ability1.isUsed = false;
            }

            return;

          }
        if (TargetedUnit.team != team && PathFinder.InRange(GameManager.Instance.hexMap, this.Hex, TargetedUnit.Hex, this.GetAbility().Range))
        {
            Stats.Energy -= 1;
            healthBar.showEnergy(Stats.Energy);
            this.GetAbility().setCoolDown(this.GetAbility().maxCoolDown);
            this.gameObject.transform.LookAt(TargetedUnit.gameObject.transform);
            this.animator.SetBool("Ability", true);
            spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
            spellAudio.Play();
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
                newStats = new CharacterStats
                {
                    MaxHealth = 1100,
                    Health = 1100 * healthPercent,

                    Damage = 100,

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
                    MaxHealth = 2200,
                    Health = 2200 * healthPercent,

                    Damage = 200,

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
        GameObject missileGO = GameObject.Instantiate(attackGO, attackPosition.position, Quaternion.identity);
        Missile missile = missileGO.GetComponent<Missile>();
        missile.moveSpeed = 5;
        missile.enemy = TargetedUnit;
        missile.DamageToDeal = this.Stats.Damage;

    }
    public void Drain()
    {
        ability1.useAbility(this);
        ability1.isUsed = false;
        this.animator.SetBool("Ability", false);
        ChangeState(IdleState);

    }

}

public class LifeDrain : Ability
{
    public GameObject Skull;
    public Transform spawnPosition;
    public const string name = "LifeDrain";
    public const int cooldown = 1;

    public const string description = "Default";
    public const string abilityType = "Magical";
    public const bool istargatable = true;
    public const float quantity = 1;
    public const int range = 2;

    public LifeDrain(GameObject skull, Transform position) : base(name, quantity, cooldown, description, abilityType, istargatable, range)
    {
        this.Skull = skull;
        this.spawnPosition = position;
    }

    public override void useAbility(Unit unit)
    {

        if (unit.TargetedUnit.team == unit.team)
        {
            GameObject skullGO = GameObject.Instantiate(Skull, spawnPosition.position, Quaternion.identity);
            skullGO.transform.LookAt(unit.TargetedUnit.gameObject.transform);

            EnergyDrain missile = skullGO.GetComponent<EnergyDrain>();
            missile.moveSpeed = 3;
            missile.enemy = unit.TargetedUnit;
            missile.EnergyToDrain = (int)this.Quantity;
        }
        if (unit.TargetedUnit.team != unit.team)
        {
            GameObject skullGO = GameObject.Instantiate(Skull, unit.TargetedUnit.gameObject.transform.position, Quaternion.identity);
            skullGO.transform.LookAt(unit.gameObject.transform);

            EnergyDrain missile = skullGO.GetComponent<EnergyDrain>();
            missile.moveSpeed = 3;
            missile.enemy = unit;
            unit.TargetedUnit.Stats.Energy -= (int)this.Quantity;
            missile.EnergyToDrain = (int)this.Quantity;
        }

       
    }
    public override void IncreaseLevel()
    {
        this.abilityLvl++;
        switch (this.abilityLvl)
        {
            case 2:
                this.Range = 2;
                 this.Quantity = 1;
                break;
            case 3:
                this.Range = 3;
               this.Quantity = 2;
                break;

            default:
                break;
        }
    }

}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lina : Unit
{
    
    [Header("NormalAttack")]
    public Transform attackPosition;
    public GameObject attackGO;

    [Header("Ability")]
    public Transform firePosition;
    public GameObject projectil;

    private void Awake()
    {

        ability1 = new MagicBall(projectil, firePosition);

        heroImage = Resources.Load<Sprite>("Lina/Lina");
        ability1Image = Resources.Load<Sprite>("Lina/Ability1");
    }
    private void Start()
    {
       
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

        Damage = 55,

        Energy = 3,
        AttackRange = 3,

        Armor = 5,
        MagicResist = 10


    };
    public override string AttackType { get; protected set; } = "Range";
    public override string Type { get; protected set; } = "Intelligence";

    public override void Ability()
    {
        if (TargetedUnit != null )
        {
            if (TargetedUnit.team != team && PathFinder.InRange(GameManager.Instance.hexMap, this.Hex, TargetedUnit.Hex, this.GetAbility().Range))
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
                this.ability1.isUsed = false;
            }

           
        }
        else
        {
            this.ChangeState(IdleState);
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

                    Damage = 105,

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

                    Damage = 205,

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

    public void Missile()
    {
        GetAbility().useAbility(TargetedUnit);
        this.ability1.isUsed = false;
        this.animator.SetBool("Ability", false);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }
    public void normalAttack()
    {
        GameObject missileGO = GameObject.Instantiate(attackGO, attackPosition.position, Quaternion.identity);
        Missile missile = missileGO.GetComponent<Missile>();
       
        missile.enemy = TargetedUnit;
        missile.DamageToDeal = this.Stats.Damage;


    }



}

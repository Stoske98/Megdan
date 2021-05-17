using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutral : Unit
{
    private void Awake()
    {
        heroImage = Resources.Load<Sprite>("Torrel/Ability1");
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
        MaxHealth = 300,
        Health = 300,

        Damage = 100,

        Energy = 3,
        AttackRange = 1,

        Armor = 0,
        MagicResist = 0


    };

    public override string AttackType { get; protected set; } = "Melee";
    public override string Type { get; protected set; } = "Neutral";

    public override void IncreaseLevel()
    {
        this.Level++;
        CharacterStats newStats;
        float healthPercent = Stats.Health / Stats.MaxHealth;
        switch (this.Level)
        {
            case 2:
                newStats = new CharacterStats
                {
                    MaxHealth = 600,
                    Health = 600 * healthPercent,

                    Damage = 200,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 0,
                    MagicResist = 0
                };
                Stats = newStats;
                break;

            case 3:
                newStats = new CharacterStats
                {
                    MaxHealth = 1200,
                    Health = 1200 * healthPercent,

                    Damage = 300,

                    Energy = 3,
                    AttackRange = 1,

                    Armor = 0,
                    MagicResist = 0
                };
                Stats = newStats;
                break;

        }

    }

    public override void setEndOfAttack()
    {
        this.animator.SetBool("Attack", false);
        this.TargetedUnit.RecieveDmg(this.Stats.Damage);
        this.TargetedUnit = null;
        ChangeState(IdleState);
    }

}

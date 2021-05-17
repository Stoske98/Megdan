using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrain : MonoBehaviour
{
    [HideInInspector]
    public Unit enemy;
    [HideInInspector]
    public int EnergyToDrain;

    private GameObject enemyGO;
    public float moveSpeed;

    void Start()
    {
        enemyGO = enemy.gameObject;
        gameObject.transform.LookAt(enemyGO.transform.position + new Vector3(0, 0.8f, 0));
    }


    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();
        if (unit != null)
        {
            if (unit == enemy)
            {
                
                unit.Stats.Energy += EnergyToDrain;
                if (unit.Stats.Energy > 3)
                {
                    unit.Stats.Energy = 3;
                }
                GameManager.Instance.updateUnitStats(unit);
                Destroy(this);
                Destroy(gameObject);
            }
        }


    }
}

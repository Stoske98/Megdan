using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{

    [HideInInspector]
    public Unit enemy;
    [HideInInspector]
    public float DamageToDeal;

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
                unit.RecieveDmg(DamageToDeal);
                GameManager.Instance.updateUnitStats(unit);
                Destroy(this);
                Destroy(gameObject);
            }
        }


    }
}

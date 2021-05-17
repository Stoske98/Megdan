using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGO : MonoBehaviour
{

    [HideInInspector]
    public Unit enemy;
    [HideInInspector]
    public float DamageToDeal;

    private GameObject enemyGO;
    private Vector3 direction;
    public float moveSpeed;

    public ParticleSystem ps;
    private Renderer r;

    private bool hit = false;

    private AudioSource spellAudio;


    void Start()
    {
        spellAudio = gameObject.GetComponent<AudioSource>();
        
        enemyGO = enemy.gameObject;
        gameObject.transform.LookAt(enemyGO.transform.position + new Vector3(0, 0.8f, 0));
        r = this.gameObject.GetComponent<Renderer>();

    }


    void Update()
    {
        if (enemyGO == null)
        {
            Destroy(gameObject);
        }

        if (!hit)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        
       
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();
        if (unit != null)
        {
            if (unit == enemy )
            {
                hit = true;
                spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
                spellAudio.Play();
                unit.RecieveMagicDmg(DamageToDeal);
                GameManager.Instance.updateUnitStats(unit);
                r.enabled = false;
                ps.Play();
                Destroy(gameObject,3);
            }
        }
       
        
    }
}

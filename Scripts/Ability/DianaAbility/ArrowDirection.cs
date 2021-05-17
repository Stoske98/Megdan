using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    [HideInInspector]
    public Unit diana;
    [HideInInspector]
    public float DamageToDeal;
    Transform dianaTR;
    public float moveSpeed;
    private AudioSource spellAudio;


    void Start()
    {
        dianaTR = diana.gameObject.transform;
        spellAudio = gameObject.GetComponent<AudioSource>();
        spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
        spellAudio.Play();

    }

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if ((this.transform.position - dianaTR.position).magnitude > 30)
        {
            Destroy(this);
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();
        if (unit != null)
        {
            if (other.isTrigger == false)
            {
                if (unit.team != diana.team)
                {
                    unit.RecieveMagicDmg(DamageToDeal);
                    GameManager.Instance.updateUnitStats(unit);
                    DamageToDeal /= 2;

                }
            }
            
            
        }


    }
}

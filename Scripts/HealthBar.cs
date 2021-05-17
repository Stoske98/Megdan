using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public GameObject[] energy;
  
    public void showEnergy(int i)
    {
        if (i == 0)
        {
            energy[0].SetActive(false);
            energy[1].SetActive(false);
            energy[2].SetActive(false);
        }else if(i == 1)
        {
            energy[0].SetActive(true);
            energy[1].SetActive(false);
            energy[2].SetActive(false);
        }else if(i == 2)
        {
            energy[0].SetActive(true);
            energy[1].SetActive(true);
            energy[2].SetActive(false);
        }
        else
        {
            energy[0].SetActive(true);
            energy[1].SetActive(true);
            energy[2].SetActive(true);
        }
    }

    public void setMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHealth(float health)
    {
        slider.value = health;
    }
}

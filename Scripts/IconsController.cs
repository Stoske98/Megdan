using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconsController : MonoBehaviour
{
    [HideInInspector]
    public Sprite sprite;
    private Image[] images;

    void Start()
    {
        images = gameObject.GetComponentsInChildren<Image>();
        
    }

    void Update()
    {
        if (sprite != null)
        {
            if (images[0].type == Image.Type.Filled)
            {
                if (images[0].fillAmount <= 1)
                {
                    images[0].fillAmount += 3 * Time.deltaTime;
                }

            }
            if (images[1].type == Image.Type.Filled)
            {
                if (images[1].fillAmount <= 1)
                {
                    images[1].sprite = sprite;
                    images[1].fillAmount += 3 * Time.deltaTime;
                    
                }
            }
            if (images.Length > 2)
            {
                if (images[2].type == Image.Type.Filled)
                {
                    if (images[2].fillAmount <= 1)
                    {
                        images[2].sprite = sprite;
                        images[2].fillAmount += 3 * Time.deltaTime;
                    }
                }
            }
        }   
    }
}

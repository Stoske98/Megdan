using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class ShowDescrition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string heroName;
    public string Description;

    private GameObject descriptionHolder;
    private RectTransform thisRectTransform;
    private TextMeshProUGUI[] TextMesh;
    private Vector3 offsetDescription = new Vector3(30,30,0);
    void Start()
    {
        heroName = this.gameObject.name;
        descriptionHolder = GameManager.Instance.description;
        TextMesh = descriptionHolder.GetComponentsInChildren<TextMeshProUGUI>();
        thisRectTransform = descriptionHolder.GetComponent<RectTransform>();


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionHolder.SetActive(true);

        thisRectTransform.position = eventData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>().position + offsetDescription;
        TextMesh[0].text = Description;
        TextMesh[1].text = heroName;
    }
    public void OnPointerExit(PointerEventData data)
    {
        descriptionHolder.SetActive(false);
    }
}

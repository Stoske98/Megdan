using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseDragHero : MonoBehaviour
{
    private HexMap hexMap;
    [HideInInspector]
    public Camera uiCamera;
    float distance_to_screen;
    Vector3 pos_move;
    [HideInInspector]
    public Vector3 startPos;
    bool startMoving = false;
    float cursorPosY = -70;
    public GameObject prefabToInstantiate;
    public int Cost;
    private Canvas canvas;
    private Text textCost;
    private RectTransform rectTransform;

    void Start()
    {
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        hexMap = GameObject.Find("HexMap").GetComponent<HexMap>();
        canvas = gameObject.GetComponentInChildren<Canvas>();
        textCost = gameObject.GetComponentInChildren<Text>();
        textCost.text = Cost.ToString() + "x";
       
        startPos = this.transform.localPosition;
       
    }

    private void OnMouseDown()
    {

        GameManager.Instance.player1.spawnUnit(Cost);
        GameManager.Instance.player2.spawnUnit(Cost);
        canvas.enabled = false;

    }
    private void OnMouseDrag()
    {
        if ((startPos - transform.localPosition).magnitude > 1)
        {
            if (!startMoving)
            {
                gameObject.transform.localRotation = Quaternion.Euler(new Vector3(135, 0, 180));
                gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                startMoving = true;
                cursorPosY = -20f;
            }
        }
        if ((startPos - transform.localPosition).magnitude < 1)
        {
            if (startMoving)
            {
                gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                startMoving = false;
                cursorPosY = -50f;
            }

        }
        distance_to_screen = uiCamera.WorldToScreenPoint(gameObject.transform.position).z;
        pos_move = uiCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y + cursorPosY, distance_to_screen));
        transform.position = new Vector3(pos_move.x, pos_move.y, pos_move.z);
    }
    private void OnMouseUp()
    {
        canvas.enabled = true;
        GameManager.Instance.player1.DropHero(prefabToInstantiate, this.gameObject, Cost);
        GameManager.Instance.player2.DropHero(prefabToInstantiate, this.gameObject, Cost);
        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        transform.localPosition = startPos;
        startMoving = false;
        cursorPosY = -70;
        GameManager.Instance.hexMap.ResetGrid();
    }

    public void resetPos()
    {
        canvas.enabled = true;
        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        transform.localPosition = startPos;
        startMoving = false;
        cursorPosY = -70;
    }



}

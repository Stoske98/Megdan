using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitAbility : State
{
    public static Image image;
    public static Animator anim;
    public override void Enter(Unit unit)
    {
        
    }

    public override void Execute(Unit unit)
    {
        if (!unit.GetAbility().isUsed)
        {
           
            if (unit.GetAbility().freeDirection)
            {
               
                Ray ray = GameManager.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (image == null)
                {
                    image = unit.gameObject.GetComponentInChildren<Image>();
                    anim = unit.gameObject.GetComponent<Animator>();
                    if (image != null)
                    {
                        unit.spellAudio.volume = SettingsControll.Instance.audioSliderEff.value;
                        unit.spellAudio.Play();
                        image.enabled = true;
                        anim.SetBool("Ability", true);
                        
                    }
                }
               
                
                
                if (Physics.Raycast(ray, out hit))
                {

                    Vector3 target = hit.point;

                    if ((target - unit.gameObject.transform.position).magnitude > 0.5f)
                    {
                        unit.gameObject.transform.LookAt(new
                        Vector3(target.x, unit.gameObject.transform.position.y, target.z));
                    }

                }
                if (Input.GetMouseButtonDown(0))
                {
                    image.enabled = false;
                    image = null;
                    anim = null;

                    unit.GetAbility().isUsed = true;
                    unit.Ability();
                }
                
                
            }else if (unit.GetAbility().isTargetable)
            {
                PathFinder.BFS_ShowRange(GameManager.Instance.hexMap, unit.Hex, unit.GetAbility().Range, Color.cyan);

                if (Input.GetMouseButtonDown(0))
                {
                    if (unit.GetAbility().HexIsTargetable)
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.collider != null)
                            {
                                if (hit.collider.gameObject.tag == "Hex")
                                {
                                    Hex hex = GameManager.Instance.hexMap.listHex[hit.collider.gameObject.transform.parent.gameObject];
                                    if (hex.Walkable)
                                    {
                                        unit.desiredHex = hex;
                                        unit.GetAbility().isUsed = true;
                                        unit.Ability();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.collider != null)
                            {
                                if (hit.collider.gameObject.GetComponent<Unit>() != null)
                                {

                                    if (unit.team == hit.collider.gameObject.GetComponent<Unit>().team)
                                    {
                                        if (GameManager.Instance.player1.team == unit.team)
                                        {
                                            GameManager.Instance.player1.selectedUnit = unit;
                                            GameManager.Instance.UpdateGUI(GameManager.Instance.player1,unit);
                                        }
                                        else if(GameManager.Instance.player2.team == unit.team)
                                        {
                                            GameManager.Instance.player2.selectedUnit = unit;
                                            GameManager.Instance.UpdateGUI(GameManager.Instance.player2, unit);
                                        }
                                        unit.TargetedUnit = hit.collider.gameObject.GetComponent<Unit>();
                                        unit.GetAbility().isUsed = true;
                                        unit.Ability();
                                    }
                                    else
                                    {
                                        if (GameManager.Instance.player1.team == unit.team)
                                        {
                                            GameManager.Instance.player1.selectedUnit = unit;
                                            GameManager.Instance.UpdateGUI(GameManager.Instance.player1, unit);
                                        }
                                        else if (GameManager.Instance.player2.team == unit.team)
                                        {
                                            GameManager.Instance.player2.selectedUnit = unit;
                                            GameManager.Instance.UpdateGUI(GameManager.Instance.player2, unit);
                                        }
                                        unit.TargetedUnit = hit.collider.gameObject.GetComponent<Unit>();
                                        unit.GetAbility().isUsed = true;
                                        unit.Ability();
                                    }

                                }
                            }
                        }
                    }

                }
            }
            else
            {

                unit.GetAbility().isUsed = true;
                unit.Ability();

            }
        }
        
    }

    public override void Exit(Unit unit)
    {
    }
}





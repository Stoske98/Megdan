using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Game : GameState
{

    
    public override void Enter()
    {
       
    }

    public override void Execute()
    {
        
        TurnControll();
        GameManager.Instance.player1.Update();
        GameManager.Instance.player2.Update();



    }

    public override void Exit()
    {
       
    }

    public void Timer()
    {
        GameManager.Instance.timeText.text = ((int)GameManager.Instance.time).ToString();
    }
    public void TurnControll()
    {
        if (0 <= GameManager.Instance.time)
        {
            Timer();
            GameManager.Instance.time -= Time.deltaTime;

        }

        if (0 >= GameManager.Instance.time && (GameManager.Instance.player1.turn || GameManager.Instance.player1.rotate))
        {
           
            if (GameManager.Instance.player1.turn)
            {
                GameManager.Instance.endturn.interactable = false;
                GameManager.Instance.hexMap.clearIndicators();
                GameManager.Instance.hexMap.ResetGrid();

                GameManager.Instance.player1.turn = false;
                GameManager.Instance.player1.rotate = true;

                GameManager.Instance.showGUI.SetBool("ShowHeroStats", false);

                foreach (var dd in GameManager.Instance.player1.dragdrop)
                {
                    dd.GetComponent<MouseDragHero>().resetPos();
                    dd.SetActive(false);
                }
                foreach (var unit in GameManager.Instance.player1.squad)
                {
                    unit.turnOnField++;
                    controllColdownSpells(unit);
                    controllUnitLevels(unit.turnOnField, unit);
                    unit.Stats.Energy = 0;
                    unit.healthBar.showEnergy(unit.Stats.Energy);
                }
            }
            if (GameManager.Instance.CameraHolder.rotation.eulerAngles.y >= 0 && GameManager.Instance.CameraHolder.rotation.eulerAngles.y <= 180)
            {
                GameManager.Instance.CameraHolder.eulerAngles = new Vector3(GameManager.Instance.CameraHolder.rotation.eulerAngles.x, GameManager.Instance.CameraHolder.rotation.eulerAngles.y + Time.deltaTime * 50, GameManager.Instance.CameraHolder.rotation.eulerAngles.z);
                return;
            }


            GameManager.Instance.time = GameManager.Instance.GameTimer;
            GameManager.Instance.player1.rotate = false;
            GameManager.Instance.player2.turn = true;
            GameManager.Instance.CameraHolder.eulerAngles = new Vector3(0, -180, 0);
          

            foreach (var unit in GameManager.Instance.player2.squad)
            {
                unit.Stats.Energy = 3;
                unit.healthBar.showEnergy(unit.Stats.Energy);
            }
            GameManager.Instance.numberOfMoves++;
            
            foreach (var dd in GameManager.Instance.player2.dragdrop)
            {
                dd.SetActive(true);
            }
            if (GameManager.Instance.player2.gems < 10 && GameManager.Instance.numberOfMoves > 1)
            {
                GameManager.Instance.player2.gems++;
            }

            GameManager.Instance.gemsText.text = GameManager.Instance.player2.gems.ToString() + "x";
            GameManager.Instance.endturn.interactable = true;
            GameManager.Instance.player1.selectedUnit = null;

        }
        if (0 >= GameManager.Instance.time && (GameManager.Instance.player2.turn || GameManager.Instance.player2.rotate))
        {
            
            if (GameManager.Instance.player2.turn)
            {
                GameManager.Instance.endturn.interactable = false;
                GameManager.Instance.hexMap.clearIndicators();
                GameManager.Instance.hexMap.ResetGrid();

                GameManager.Instance.player2.turn = false;
                GameManager.Instance.player2.rotate = true;

                GameManager.Instance.showGUI.SetBool("ShowHeroStats", false);

                foreach (var dd in GameManager.Instance.player2.dragdrop)
                {
                    dd.GetComponent<MouseDragHero>().resetPos();
                    dd.SetActive(false);
                }
                foreach (var unit in GameManager.Instance.player2.squad)
                {
                    unit.turnOnField++;
                    controllColdownSpells(unit);
                    controllUnitLevels(unit.turnOnField, unit);
                    unit.Stats.Energy = 0;
                    unit.healthBar.showEnergy(unit.Stats.Energy);
                }
            }
            if (GameManager.Instance.CameraHolder.rotation.eulerAngles.y < 360 && GameManager.Instance.CameraHolder.rotation.eulerAngles.y >= 180)
            {
                GameManager.Instance.CameraHolder.eulerAngles = new Vector3(GameManager.Instance.CameraHolder.rotation.eulerAngles.x, GameManager.Instance.CameraHolder.rotation.eulerAngles.y + Time.deltaTime * 50, GameManager.Instance.CameraHolder.rotation.eulerAngles.z);
                return;
            }

            GameManager.Instance.time = GameManager.Instance.GameTimer;
            GameManager.Instance.player1.turn = true;
            GameManager.Instance.player2.rotate = false;
            GameManager.Instance.CameraHolder.eulerAngles = new Vector3(0,0,0);
           

            foreach (var unit in GameManager.Instance.player1.squad)
            {
                unit.Stats.Energy = 3;
                unit.healthBar.showEnergy(unit.Stats.Energy);
            }
            GameManager.Instance.numberOfMoves++;

            foreach (var dd in GameManager.Instance.player1.dragdrop)
            {
                dd.SetActive(true);
            }
            if (GameManager.Instance.player1.gems < 10)
            {
                GameManager.Instance.player1.gems++;
            }

            GameManager.Instance.gemsText.text = GameManager.Instance.player1.gems.ToString() + "x";
            GameManager.Instance.endturn.interactable = true;
            GameManager.Instance.player2.selectedUnit = null;
        }
    }

  

    public void controllColdownSpells(Unit unit)
    {
        if (unit.ability1 != null)
        {
             if (unit.ability1.CoolDown != 0 && !unit.ability1.isPassive)
        {
            unit.ability1.setCoolDown(unit.ability1.getCoolDown() - 1);
        }
        }
       
        if (unit.ability2 != null)
        {
            if (unit.ability2.CoolDown != 0 && !unit.ability2.isPassive)
            {
                unit.ability2.setCoolDown(unit.ability2.getCoolDown() - 1);
            }
        }
        if (unit.ability3 != null)
        {
            if (unit.ability3.CoolDown != 0 && !unit.ability3.isPassive)
            {
                unit.ability3.setCoolDown(unit.ability3.getCoolDown() - 1);
            }
        }
       

    }
    public void controllUnitLevels(int turnonfield, Unit unit)
    {
        switch (turnonfield)
        {
            case 2:
                GameObject lvlup1 = GameObject.Instantiate(GameManager.Instance.LevelUpParticle, unit.transform.position, Quaternion.identity);
                GameObject.Destroy(lvlup1, 3);
                unit.IncreaseLevel();
                break;
            case 4:
                GameObject lvlup2 = GameObject.Instantiate(GameManager.Instance.LevelUpParticle, unit.transform.position, Quaternion.identity);
                GameObject.Destroy(lvlup2, 3);
                unit.IncreaseLevel();
                break;
         

            default:
                break;

        }

    }
}






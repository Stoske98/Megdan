using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    public List<GameObject> dragdrop = new List<GameObject>();
    public List<Unit> squad = new List<Unit>();
    public int gems;
    public int team;
    public bool turn;
    public bool rotate = false;
    public HexMap hexMap;
    public Unit selectedUnit;
    private List<Hex> spawnablePosition;
    private List<Hex> startSpawnablePosition;
    public Hex startHex;

    public Player(List<Unit> units, int whichTeam, bool isStartFirst, HexMap map)
    {
        this.squad = units;
        this.team = whichTeam;
        this.turn = isStartFirst;
        this.gems = 3;
        this.hexMap = map;
        spawnablePosition = new List<Hex>();
        startSpawnablePosition = new List<Hex>();
        if (team == 1)
        {
            startHex = hexMap.GetHex(1,5);
            foreach (var neighbor in hexMap.GetNeighbors(startHex))
            {
                startSpawnablePosition.Add(neighbor);
            }
            startSpawnablePosition.Add(startHex);
        }
        if (team == 2)
        {
            startHex = hexMap.GetHex(9, 5);
            foreach (var neighbor in hexMap.GetNeighbors(startHex))
            {
                startSpawnablePosition.Add(neighbor);
            }
            startSpawnablePosition.Add(startHex);
        }
        
    }

    public void Update()
    {
        if (turn && !rotate )
        {
            SelectHero();
            if (selectedUnit != null && selectedUnit.team == team)
            {
                
                ShowPath();
                MoveTheUnit();
                AttackUnit();
                useAbility();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                hexMap.ResetGrid();
                for (int i = 0; i < hexMap.hexes.Length - 1; i++)
                {
                    if (hexMap.hexes[i].Walkable == false)
                    {
                        hexMap.hexes[i].SetColor(Color.black);
                    }
                }
            }

           
        }
      
        
    }

    public void useAbility()
    {
        if (Input.GetKeyDown(KeyCode.Q) && selectedUnit != null && selectedUnit.ability1 != null && selectedUnit.currentState == selectedUnit.IdleState && selectedUnit.team == team  && !selectedUnit.ability1.isPassive)
        {
            if (selectedUnit.Stats.Energy >= 1)
            {
                if (selectedUnit.ability1.CoolDown == 0)
                {
                    hexMap.clearIndicators();
                    hexMap.ResetGrid();
                    selectedUnit.setAbility(selectedUnit.ability1); 
                    selectedUnit.ChangeState(selectedUnit.abilityState);
                    selectedUnit.TargetedUnit = null;

                }

            }
        }

        if (Input.GetKeyDown(KeyCode.E) && selectedUnit != null && selectedUnit.ability3 != null && !selectedUnit.ability3.isPassive && selectedUnit.currentState == selectedUnit.IdleState && selectedUnit.team == team)
        {
            if (selectedUnit.Stats.Energy >= 1)
            {
                if (selectedUnit.ability3.CoolDown == 0)
                {
                    hexMap.clearIndicators();
                    hexMap.ResetGrid();
                    selectedUnit.setAbility(selectedUnit.ability3);
                    selectedUnit.ChangeState(selectedUnit.abilityState);
                    selectedUnit.TargetedUnit = null;

                }

            }
        }
    }
    private void ShowPath()
    {
        if (selectedUnit != null && selectedUnit.currentState == selectedUnit.IdleState)
        {
            RaycastHit Hit;
            Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(Ray, out Hit))
            {
                if (Hit.collider != null)
                {
                    foreach (Hex t in hexMap.hexes)
                    {

                        if (Hit.collider.gameObject.transform.parent.gameObject != null)
                        {
                            if (t.getGO() == Hit.collider.gameObject.transform.parent.gameObject)
                            {
                                showPath(selectedUnit.Hex, t);
                            }
                        }
                       
                        
                    }
                }
            }

             walkabelTiles(selectedUnit);
        }
    }
    private void MoveTheUnit()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnit != null && selectedUnit.currentState == selectedUnit.IdleState) {
            RaycastHit Hit;
            Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(Ray, out Hit))
            {
                if (Hit.collider != null)
                {
                    foreach (Hex t in hexMap.hexes)
                    {
                        if (t.getGO() == Hit.collider.gameObject.transform.parent.gameObject && t.Walkable)
                        {
                            selectedUnit.desiredHex = t;
                            selectedUnit.ChangeState(selectedUnit.moveState);
                        }
                    }
                }
            }
        }

    }

    private void SelectHero()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<Unit>() != null)
                    {
                        
                        if(this.team == hit.collider.gameObject.GetComponent<Unit>().team)
                        {

                            if (selectedUnit == null)
                            {
                                GameManager.Instance.showGUI.SetBool("ShowHeroStats", true);
                            }
                            selectedUnit = hit.collider.gameObject.GetComponent<Unit>();
                            GameManager.Instance.UpdateGUI(this, selectedUnit);
                           
                            
                        }
                        else
                        {

                            if (selectedUnit == null)
                            {
                                GameManager.Instance.showGUI.SetBool("ShowHeroStats", true);
                            }
                            GameManager.Instance.hexMap.clearIndicators();
                            GameManager.Instance.hexMap.ResetGrid();
                            selectedUnit = hit.collider.gameObject.GetComponent<Unit>();
                            GameManager.Instance.UpdateGUI(this, selectedUnit);
                        }

                    }
                }
            }
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (selectedUnit != null && selectedUnit.currentState == selectedUnit.abilityState)
            {
                
                    if (selectedUnit.ability1 != null && !selectedUnit.ability1.isUsed)
                    {
                        if (UnitAbility.image != null)
                        {
                            UnitAbility.image.enabled = false;
                            UnitAbility.image = null;
                            GameObject.FindWithTag("Arrow").SetActive(false);
                            UnitAbility.anim.SetBool("Ability", false);
                            UnitAbility.anim = null;
                        }
                        selectedUnit.ChangeState(selectedUnit.IdleState);
                    }
                    if (selectedUnit.ability2 != null && !selectedUnit.ability2.isUsed)
                    {
                        selectedUnit.ChangeState(selectedUnit.IdleState);
                    }
                    if (selectedUnit.ability3 != null && !selectedUnit.ability3.isUsed)
                    {
                        selectedUnit.ChangeState(selectedUnit.IdleState);
                    }
                
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.tag == "Cube")
                        {
                            hexMap.clearIndicators();
                            hexMap.ResetGrid();
                            
                        }
                    }
                }
                
            }else if(selectedUnit != null && selectedUnit.currentState == selectedUnit.IdleState)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.tag == "Cube")
                        {

                            hexMap.clearIndicators();
                            hexMap.ResetGrid();
                            selectedUnit = null;
                            GameManager.Instance.showGUI.SetBool("ShowHeroStats", false);
                        }
                    }
                }
            }
        }

    }

    private void AttackUnit()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnit != null && selectedUnit.currentState == selectedUnit.IdleState)
        {
           
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<Unit>() != null)
                    {

                        if (this.team != hit.collider.gameObject.GetComponent<Unit>().team)
                        {
                            selectedUnit.TargetedUnit = hit.collider.gameObject.GetComponent<Unit>();
                            selectedUnit.readyToAttack = true;
                            selectedUnit.ChangeState(selectedUnit.attackState);
                        }
                       

                    }
                }
            }
        }
    }

    public void showPath(Hex start, Hex end)
    {
        Hex[] path = PathFinder.FindPath_AStar(hexMap, start, end).ToArray();
        int indic = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            int col = path[i + 1].Col - path[i].Col;
            int row = path[i + 1].Row - path[i].Row;

            if (row == 1 && col == 0)
            {
                hexMap.indicators[indic].transform.rotation = Quaternion.Euler(Vector3.zero);
                hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                hexMap.indicators[indic].transform.position = path[i].getGO().transform.position;
                indic++;
            }
            if (row == -1 && col == 0)
            {
                hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                hexMap.indicators[indic].transform.position = path[i].getGO().transform.position;
                indic++;
            }
            if (path[i].Col % 2 == 0)
            {

                if (row == 0 && col == -1)
                {
                    hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, -60, 0));
                    hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    hexMap.indicators[indic].transform.position = path[i].getGO().transform.position + Vector3.zero;
                    indic++;
                }
                else if (row == 0 && col == 1)
                {
                    hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, 60, 0));
                    hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    hexMap.indicators[indic].transform.position = path[i].getGO().transform.position + Vector3.zero;
                    indic++;
                }
                else if (row == -1 && col == -1)
                {
                    hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, -120, 0));
                    hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    hexMap.indicators[indic].transform.position = path[i].getGO().transform.position + Vector3.zero;
                    indic++;
                }
                else if (row == -1 && col == 1)
                {
                    hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, 120, 0));
                    hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    hexMap.indicators[indic].transform.position = path[i].getGO().transform.position + Vector3.zero;
                    indic++;
                }
            }
            else
            {
                if (row == 1 && col == -1)
                {
                    hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, -60, 0));
                    hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    hexMap.indicators[indic].transform.position = path[i].getGO().transform.position + Vector3.zero;
                    indic++;
                }
                else if (row == 1 && col == 1)
                {
                    hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, 60, 0));
                    hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    hexMap.indicators[indic].transform.position = path[i].getGO().transform.position + Vector3.zero;
                    indic++;
                }
                else if (row == 0 && col == -1)
                {
                    hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, -120, 0));
                    hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    hexMap.indicators[indic].transform.position = path[i].getGO().transform.position + Vector3.zero;
                    indic++;
                }
                else if (row == 0 && col == 1)
                {
                    hexMap.indicators[indic].transform.rotation = Quaternion.Euler(new Vector3(0, 120, 0));
                    hexMap.indicators[indic].GetComponentInChildren<SpriteRenderer>().enabled = true;
                    hexMap.indicators[indic].transform.position = path[i].getGO().transform.position + Vector3.zero;
                    indic++;
                }
            }

        }
        if (selectedUnit.Stats.Energy >= path.Length-1)
        {
            path[path.Length - 1].SetColor(hexMap.TileColor_Start);
        }
        else
        {
            path[path.Length - 1].SetColor(hexMap.TileColor_End);
        }
       
    }

    public void clearIndicators()
    {
        foreach (var indicator in hexMap.indicators)
        {
            indicator.GetComponentInChildren<SpriteRenderer>().enabled = false;

        }
    }

    public void walkabelTiles(Unit unit)
    {
        PathFinder.BFS_ShowRange(hexMap, unit.Hex, unit.Stats.Energy, hexMap.TileColor_Frontier);
        
    }

    public void DropHero(GameObject hero, GameObject heroDragAndDrop, int Cost)
    {
        if (turn && gems >= Cost)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.tag == "Hex" )
                {
                    
                    Hex hex = hexMap.listHex[hit.collider.gameObject.transform.parent.gameObject];
                    if (hex.Walkable && spawnablePosition.Contains(hex))
                    {
                        gems -= Cost;
                        GameManager.Instance.gemsText.text = gems.ToString()+"x";

                        GameObject unitGo = GameObject.Instantiate(hero, new Vector3(0, 0, 0), Quaternion.identity);
                        if (team == 2)
                        {
                            unitGo.transform.Rotate(new Vector3(0,180,0));
                        }
                        Unit unit = unitGo.GetComponent<Unit>();
                        if (unit != null)
                        {
                            squad.Add(unit);
                        }
                        unit.tileCol = hex.Col;
                        unit.tileRow = hex.Row;
                        unit.team = this.team;
                        if (selectedUnit == null)
                        {
                            GameManager.Instance.showGUI.SetBool("ShowHeroStats", true);
                        }
                        selectedUnit = unit;
                        GameManager.Instance.UpdateGUI(this, selectedUnit);
                        dragdrop.Remove(heroDragAndDrop);
                        GameObject.Destroy(heroDragAndDrop);
                    }
                }
            }
        }
    }

    public void spawnUnit(int Cost)
    {
        if (turn && gems >= Cost)
        {
            selectedUnit = null;
            spawnablePosition.Clear();

            if (team == 1)
            {
                foreach (var flag in GameManager.Instance.flags)
                {
                    if (flag.team == Flag.Occupied.Blue)
                    {
                        foreach (var f in flag.neighbors)
                        {
                            spawnablePosition.Add(f);
                            f.SetColor(Color.blue);
                        }

                    }
                }
                foreach (var start in startSpawnablePosition)
                {
                    spawnablePosition.Add(start);
                    start.SetColor(Color.blue);
                }
                return;
            }
            if (team == 2)
            {
                foreach (var flag in GameManager.Instance.flags)
                {
                    if (flag.team == Flag.Occupied.Red)
                    {
                        foreach (var f in flag.neighbors)
                        {
                            spawnablePosition.Add(f);
                            f.SetColor(Color.red);
                        }

                    }
                }
                foreach (var start in startSpawnablePosition)
                {
                    spawnablePosition.Add(start);
                    start.SetColor(Color.red);
                }
            }
        }

    }

    

}

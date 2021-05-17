using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
public class HexMap : MonoBehaviour
{

    private const int TileWeight_Default = 1;
    private const int TileWeight_Expensive = 50;
    private const int TileWeight_NotWalkable = 100;
    private const int TileWeight_Infinity = int.MaxValue;

    private int Rows = 11;
    private int Cols = 11;
    public GameObject HexPrefab;
    public GameObject IndicatorPrefab;

    public Color TileColor_Default = new Color(0.86f, 0.83f, 0.83f);
    public Color TileColor_Expensive = new Color(0.19f, 0.65f, 0.43f);
    public Color TileColor_Infinity = new Color(0.37f, 0.37f, 0.37f);
    public Color TileColor_Start = Color.green;
    public Color TileColor_End = Color.red;
    public Color TileColor_Path = new Color(0.73f, 0.0f, 1.0f);
    public Color TileColor_Visited = new Color(0.75f, 0.55f, 0.38f);
    public Color TileColor_Frontier = new Color(0.4f, 0.53f, 0.8f);
    public Color TileColor_Wall = new Color(1f, 1f, 1f);


    public Dictionary<GameObject, Hex> listHex = new Dictionary<GameObject, Hex>();

    public Hex[] hexes { get; private set; }
    public GameObject[] indicators { get; private set; }

 
    void Awake()
    {
        hexes = new Hex[Rows * Cols];
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                Hex hex = new Hex(this, r, c, TileWeight_Default);
                hex.InitGameObject(transform, HexPrefab);
               
                int index = GetHexTileIndex(r, c);
                hexes[index] = hex;
                listHex.Add(hex.getGO(),hex);
                hex.SetColor(TileColor_Default);
              
               //hex.SetText(r.ToString() + "," + c.ToString());

            }
        }
        hexThatIsNotInGame();
        indicators = new GameObject[20];
        for (int i = 0; i < 20; i++)
        {
            indicators[i] = GameObject.Instantiate(IndicatorPrefab);
            indicators[i].name = "Indicator " + i; 
            indicators[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }

    
    public void ResetGrid()
    {
        foreach (var hex in hexes)
        {
            hex.Cost = 0;
            hex.PrevTile = null;


            switch (hex.Weight)
            {
                case TileWeight_Default:
                    hex.SetColor(TileColor_Default);
                    break;
                case TileWeight_Expensive:
                    hex.SetColor(TileColor_Expensive);
                    break;
                case TileWeight_Infinity:
                    hex.SetColor(TileColor_Infinity);
                    break;
                case TileWeight_NotWalkable:
                    hex.SetColor(TileColor_Wall);
                    break;
            }

        }
    }
 
    public IEnumerable<Hex> GetNeighbors(Hex hex)
    {
        Hex up = GetHex(hex.Row + 1, hex.Col);
        if (up != null)
        {
            yield return up;
        }
        Hex down = GetHex(hex.Row - 1, hex.Col);
        if (down != null)
        {
            yield return down;
        }
        if (hex.Col % 2 == 0)
        {

            Hex upleft = GetHex(hex.Row, hex.Col - 1);
            if (upleft != null)
            {
                yield return upleft;
            }
            Hex upright = GetHex(hex.Row, hex.Col + 1);
            if (upright != null)
            {
                yield return upright;
            }
            Hex downleft = GetHex(hex.Row - 1, hex.Col - 1);
            if (downleft != null)
            {
                yield return downleft;
            }
            Hex downright = GetHex(hex.Row - 1, hex.Col + 1);
            if (downright != null)
            {
                yield return downright;
            }

        }
        else
        {

            Hex upleft = GetHex(hex.Row + 1, hex.Col - 1);
            if (upleft != null)
            {
                yield return upleft;
            }
            Hex upright = GetHex(hex.Row + 1, hex.Col + 1);
            if (upright != null)
            {
                yield return upright;
            }
            Hex downleft = GetHex(hex.Row, hex.Col - 1);
            if (downleft != null)
            {
                yield return downleft;
            }
            Hex downright = GetHex(hex.Row, hex.Col + 1);
            if (downright != null)
            {
                yield return downright;
            }
        }

    }
    private int GetHexTileIndex(int row, int col)
    {
        return row * Cols + col;
    }
    public Hex GetHex(int row, int col)
    {
        if (!IsInBounds(row, col))
        {
            return null;
        }

        return hexes[GetHexTileIndex(row, col)];
    }
    private bool IsInBounds(int row, int col)
    {
        bool rowInRange = row >= 0 && row < Rows;
        bool colInRange = col >= 0 && col < Cols;
        return rowInRange && colInRange;
    }
    public Vector3 positionHex(Hex hex)
    {
        return hex.getGO().transform.position + new Vector3(0, 0.2f, 0);
    }

    public void hexThatIsNotInGame()
    {
        List<Hex> lista = new List<Hex>();
        lista.Add(GetHex(0,0));
        lista.Add(GetHex(1, 0));
        lista.Add(GetHex(0, 1));
        lista.Add(GetHex(0, 2));
        lista.Add(GetHex(0, 6));
        lista.Add(GetHex(0, 7));
        lista.Add(GetHex(1, 8));
        lista.Add(GetHex(1, 9));
        lista.Add(GetHex(1, 10));
        lista.Add(GetHex(0, 10));
        lista.Add(GetHex(2, 0));
        lista.Add(GetHex(1, 1));
        lista.Add(GetHex(1, 2));
        lista.Add(GetHex(0, 3));
        lista.Add(GetHex(0, 4));
        lista.Add(GetHex(0, 9));
        lista.Add(GetHex(10, 7));
        lista.Add(GetHex(1, 8));
        lista.Add(GetHex(9, 9));
        lista.Add(GetHex(9, 10));
        lista.Add(GetHex(10, 3));
        lista.Add(GetHex(10, 2));
        lista.Add(GetHex(10, 9));
        lista.Add(GetHex(10, 10));
        lista.Add(GetHex(9, 0));
        lista.Add(GetHex(9, 1));
        lista.Add(GetHex(10, 0));
        lista.Add(GetHex(10, 1));
        lista.Add(GetHex(0, 8));
        lista.Add(GetHex(10, 8));
        lista.Add(GetHex(2, 10));
        foreach (var hex in lista)
        {
            hex.Walkable = false;
            hex.getGO().SetActive(false);
        }
    }
    public Hex[] return_path(Hex start ,Hex end)
    {
        return  PathFinder.FindPath_AStar(this, start, end).ToArray();
    }
    public void clearIndicators()
    {
        foreach (var indicator in indicators)
        {
            indicator.GetComponentInChildren<SpriteRenderer>().enabled = false;

        }
    }






}

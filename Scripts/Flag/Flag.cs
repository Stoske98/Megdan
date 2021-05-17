using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{

    public int hexCol;
    public int herRow;
    private Hex hex;
    private HexMap hexMap;
    public GameObject flagColor;
    private Material material;
    public List<Hex> neighbors;

    public enum Occupied { None, Red, Blue };
    public Occupied team;
    void Start()
    {
        hexMap = GameManager.Instance.hexMap;
        hex = hexMap.GetHex(hexCol,herRow);
        this.transform.parent = hexMap.transform;
        this.transform.position = new Vector3(hex.getGO().transform.position.x, this.transform.position.y, hex.getGO().transform.position.z);
        team = Occupied.None;
        material = flagColor.GetComponent<Renderer>().material;
        neighbors = new List<Hex>();
        foreach (var neighbor in hexMap.GetNeighbors(hex))
        {
            neighbors.Add(neighbor);
        }
        neighbors.Add(hex);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() != null)
        {
            if (other.gameObject.GetComponent<Unit>().team == 1 && !hex.Walkable)
            {
                team = Occupied.Blue;
                material.mainTexture = GameManager.Instance.team2blue;

            }
            if (other.gameObject.GetComponent<Unit>().team == 2 && !hex.Walkable)
            {
                team = Occupied.Red;
                material.mainTexture = GameManager.Instance.team1red;
            }

        }
    }
}

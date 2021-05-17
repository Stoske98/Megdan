using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hex 
{
    const float R = 0.575f;
    const float KOREN = 0.8660254037844386f;//iz 3 kroz 2

    public HexMap Grid { get; private set; }
    public int Row { get; private set; }
    public int Col { get; private set; }
    public int S { get; private set; }
    public int Weight { get; set; }
    public int Cost { get; set; }

    public bool Walkable { get; set; }
    public Hex PrevTile { get; set; }

    public Unit unit { get; set; }

    public GameObject getGO() { return this._gameObject; }

    private GameObject _gameObject;
    private MeshRenderer _meshRenderer;
    private TextMeshProUGUI _textComponent;

    public Hex(HexMap grid, int row, int col, int weight)
    {
        Grid = grid;
        Row = row;
        Col = col;
        this.S = -col - row;
        Weight = weight;
        Walkable = true;
    }

    public void InitGameObject(Transform parent, GameObject hexPrefab)
    {
        _gameObject = GameObject.Instantiate(hexPrefab);
        _gameObject.name = $"Hex({Row}, {Col})";
        _gameObject.transform.parent = parent;
        _gameObject.transform.localPosition = Position();
        _gameObject.transform.localScale = new Vector3(0.9f,0.9f,0.9f);
        _meshRenderer = _gameObject.GetComponentInChildren<MeshRenderer>();
        _textComponent = _gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetColor(Color color)
    {
        _meshRenderer.material.color = color;
    }

    public void SetText(string text)
    {
        _textComponent.text = text;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(Col, 0, Row);
    }
    public Vector3 Position()
    {
        if (this.Col % 2 != 0)
        {
            return new Vector3(this.Col * HorizontalDistance(), 0, this.Row * VerticalDistance() + VerticalDistance() / 2);

        }
        else
        {
            return new Vector3(this.Col * HorizontalDistance(), 0, this.Row * VerticalDistance());
          }
    }
    public float HorizontalDistance()
    {
        return getWidth() * 0.75f;
    }
    public float VerticalDistance()
    {
        return getHeight();
    }
    public float getWidth()
    {
        return R * 2;
    }
    public float getHeight()
    {
        return KOREN * R * 2;
    }
 
}
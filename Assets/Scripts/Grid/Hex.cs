using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Hex : MonoBehaviour
{
    
    [SerializeField] private Unit unit;
    [SerializeField] private GlowHighlight highlight;
    [SerializeField] private HexType hexType;
    private HexCoordinates hexCoordinates;
    public Unit Unit {get => unit;  set{unit = value;}}
    public Vector3Int HexCoordinates => hexCoordinates.GetHexCoords();
    private void Awake() {
        hexCoordinates = GetComponent<HexCoordinates>();
        highlight = GetComponent<GlowHighlight>();
    }
    public int GetCost()
        =>hexType switch
        {
            HexType.Difficult => 2,
            HexType.Default => 1,
            HexType.None => 1,
            HexType.Road => 1,
            _ => throw new System.Exception("Hex type not supported")
        };
    public bool IsObstacle()
    {
        return this.hexType == HexType.Obstacle;
    }
    public bool IsEnemy()
    {
        return ( unit != null && Unit.Side == Side.Enemy);
    }
    internal void ResetHighlight()
    {
        highlight.ResetGlowHighlight();
    }
    internal void HighlightPath()
    {

        highlight.HighlightValidPath();
    }
    public void EnableHighligh(bool IsEnemy = false)
    {
        

        highlight.ToggleGlow(true);
    }
    public void DisableHighligh(bool IsEnemy = false)
    {
        
        highlight.ToggleGlow(false);
    }
}
public enum HexType
{
    None,
    Default,
    Difficult,
    Road,
    Water,
    Obstacle
}

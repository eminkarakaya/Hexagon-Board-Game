using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[SelectionBase]
public class Hex : MonoBehaviour
{
    public bool isVisible;
    [SerializeField] private Unit unit;
    [SerializeField] private Building building;
    [SerializeField] private GlowHighlight highlight;
    [SerializeField] private HexType hexType;
    private HexCoordinates hexCoordinates;
    public Building Building {get=>building;}
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
    public List<GameObject> GetLinkedObjects()
    {
        List<GameObject> list = new List<GameObject>();
        if(unit != null)
        {
            foreach (var item in unit.Sight)
            {
                list.Add(item);
            }
        }
        return list;
    }
    
    public void OpenLinkedObjectSight()
    {
        foreach (var item in GetLinkedObjects())
        {
            item.layer = LayerMask.NameToLayer("SightLayer");
        }
    }
    public void CloseLinkedObjectSight()
    {
        foreach (var item in GetLinkedObjects())
        {
            item.layer = LayerMask.NameToLayer("Default");
        }
    }
    public bool IsObstacle()
    {
        return this.hexType == HexType.Obstacle;
    }
    public bool IsEnemy()
    {
        return ( unit != null && Unit.Side == Side.Enemy);
    }
    public bool IsBuilding()
    {
        return (building != null);
    }
    internal void ResetHighlight()
    {
        highlight.ResetGlowHighlight();
    }
    internal void HighlightPath()
    {

        highlight.HighlightValidPath();
    }
    public void DisableHighlighRange()
    {
        highlight.ToggleRangeGlow(false);

    }
    public void EnableHighlighRange()
    {
        highlight.ToggleRangeGlow(true);
    }
    
    public void EnableHighlighEnemy()
    {
        highlight.ToggleEnemyGlow(true);
    }
    public void DisableHighlighEnemy()
    {
        highlight.ToggleEnemyGlow(false);
    }
    public void EnableHighligh()
    {
        highlight.ToggleGlow(true);
    }
    public void DisableHighligh()
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

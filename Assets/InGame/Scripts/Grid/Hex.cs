using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
[SelectionBase]
public class Hex : NetworkBehaviour
{
    public bool isCoast;
    public bool isVisible;
    public bool isReachable;
    public bool IsCoastCity()
    {
        if(Building != null && isCoast == true)
        {
            return true;
        }
        return false;
    }
    [SerializeField] private GlowHighlight highlight;
    [SerializeField] private HexType hexType;
    [SerializeField] private HexCoordinates hexCoordinates;
    // public Settler Settler { get; set; }
    // public Building Building {get=>building; set {building = value;}}
    // public Unit Unit {get => unit;  set{unit = value;}}
    public Vector3Int HexCoordinates => hexCoordinates.GetHexCoords();
    [SyncVar] [SerializeField] private Unit unit;
    [SyncVar] [SerializeField] private Settler settler;
    [SyncVar] [SerializeField] private Building building;
    [SyncVar] [SerializeField] private Ship ship;
    public Unit Unit { get => unit;
    
     set{unit = value;}}
    public Settler Settler { get => settler; set{settler = value;} }
    public Building Building { get => building; set{building = value;} }
    public Ship Ship { get=> ship; set{ship = value;} }
    

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
            HexType.Water => 1,
            
            _ => throw new System.Exception("Hex type not supported")
        };
    public List<GameObject> GetLinkedObjects()
    {
        List<GameObject> list = new List<GameObject>();
        if(Unit != null)
        {
            foreach (var item in Unit.Visions)
            {
                list.Add(item);
            }
        }
        if(settler != null)
        {
            foreach (var item in settler.Visions)
            {
                list.Add(item);
            }
        }
        if(building != null)
        {
            foreach (var item in building.Visions)
            {
                list.Add(item);
            }
        }
        return list;
    }
    
    public void OpenLinkedObjectVision()
    {
        foreach (var item in GetLinkedObjects())
        {
            item.layer = LayerMask.NameToLayer("SightLayer");
        }
    }
    public void CloseLinkedObjectVision()
    {
        foreach (var item in GetLinkedObjects())
        {
            item.layer = LayerMask.NameToLayer("Default");
        }
    }
    public bool IsWaterOrIsCoastCity()
    {
        return IsWater() || IsCoastCity();
    }
    public bool IsWater()
    {
        return this.hexType == HexType.Water;
    }
    public bool IsObstacle()
    {
        return this.hexType == HexType.Obstacle;
    }
    public bool IsEnemyBuilding()
    {
        return Building != null && Building.Side == Side.Enemy;
    }
    public bool IsEnemy()
    {
        return Unit != null && Unit.Side == Side.Enemy;
    }
    public bool IsMeBuilding()
    {
        return Building != null && Building.Side == Side.Me;

    }
    public bool IsEnemyShip()
    {
        return Ship != null && Ship.Side == Side.Enemy;
    }
    public bool IsMeShip()
    {
        return Ship != null && Ship.Side == Side.Me;
    }
    public bool IsAlly()
    {
        return Unit != null && Unit.Side == Side.Ally;
    }
    public bool IsAllySettler()
    {
        return Settler != null && Settler.Side == Side.Ally;
    }
    public bool IsAllyBuilding()
    {
        return Building != null && Building.Side == Side.Ally;
    }
    public bool IsMe()
    {
        return Unit != null && Unit.Side == Side.Me;
    }
    public bool IsEnemySettler()
    {
        return settler != null && settler.Side == Side.Enemy;
    }
    public bool IsMeSettler()
    {
        return settler != null && settler.Side == Side.Me;
    }
    public bool IsSettler()
    {
        return settler != null;
    }
    public bool IsBuilding()
    {
        return (building != null);
    }
    public void ResetHighlight()
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

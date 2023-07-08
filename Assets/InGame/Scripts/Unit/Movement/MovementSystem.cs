using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
public abstract class MovementSystem
{
    public int Range;
    [SerializeField] public HexGrid hexGrid;
    public float h;
    public BFSResult movementRange = new BFSResult();
    [SerializeField] protected List<Vector3Int> currentPath = new List<Vector3Int>();
    public MovementSystem(IMovable movement)
    {
        
    }
    public abstract void ShowRange(IMovable selectedUnit,Movement unit);
    
    public void HideRange(IMovable movable,Movement movement)
    {
        if(UnitManager.Instance.selectedUnit != movement.GetComponent<ISelectable>()) 
        {
            return;
        }
        if(UnitManager.Instance.selectedUnit == null) return;
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        CalculateRange(movable,hexGrid);
        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hex.DisableHighligh();
            hex.isReachable = false;
        }
        foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hex.isReachable = false;
            hex.DisableHighlighEnemy();
        }
        movementRange = new BFSResult();
    }
    
    public abstract void CalculateRange(IMovable selectedUnit,HexGrid hexGrid);
        
    public abstract void ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid,int range);
    
    public abstract void MoveUnit(Movement selectedUnit,HexGrid hexGrid, Hex hex);
  
    public bool IsHexInRange(Vector3Int start,Vector3Int hexPosition,int movementPoints)
    {
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        BFSResult result = GraphSearch.BsfGetRange(hexGrid,start,movementPoints);

        return result.IsHecPositionInRange(hexPosition);
    }
    public bool IsHexInRange(Vector3Int hexPosition)
    {
        return movementRange.IsHecPositionInRange(hexPosition);
    }
    
    
}


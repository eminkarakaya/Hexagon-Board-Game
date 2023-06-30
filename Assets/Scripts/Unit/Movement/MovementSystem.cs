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
    public void ShowRange(IMovable selectedUnit,Movement unit)
    {
        if(UnitManager.Instance.selectedUnit  == null) return;
        if(UnitManager.Instance.selectedUnit != unit.GetComponent<ISelectable>()) return;
        Debug.Log(unit,unit);
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        CalculateRange(selectedUnit,hexGrid);
        Vector3Int unitPos = hexGrid.GetClosestHex(selectedUnit.Movement.transform.position);
        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            if(unitPos == hexPosition) continue;
            hexGrid.GetTileAt(hexPosition).EnableHighligh();
        }
        foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
        {
            // hexGrid.GetTileAt(hexPosition).
            hexGrid.GetTileAt(hexPosition).EnableHighlighEnemy();
        }
    }
    

    public void HideRange(IMovable movable,Movement unit)
    {
        if(UnitManager.Instance.selectedUnit != unit.GetComponent<ISelectable>()) 
        {
            return;
        }
        if(UnitManager.Instance.selectedUnit == null) return;
        Debug.Log(unit,unit);
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        CalculateRange(movable,hexGrid);
        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            hexGrid.GetTileAt(hexPosition).DisableHighligh();
        }
        foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
        {
            hexGrid.GetTileAt(hexPosition).DisableHighlighEnemy();
        }
        movementRange = new BFSResult();
    }
    
    public abstract void CalculateRange(IMovable selectedUnit,HexGrid hexGrid);
        
    public abstract void ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid);
    
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


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
    public MovementSystem(Movement movement)
    {
        
    }
    public void ShowRange(Unit selectedUnit,Movement unit)
    {
        if(!UnitManager.Instance.selectedUnit == unit.GetComponent<Unit>()) return;

        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        CalculateRange(selectedUnit.GetComponent<Movement>(),hexGrid);
        Vector3Int unitPos = hexGrid.GetClosestHex(selectedUnit.transform.position);
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
    

    public void HideRange(Movement unit)
    {
        if(!UnitManager.Instance.selectedUnit == unit.GetComponent<Unit>()) 
        {
            return;
        }
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        // CalculateRange(selectedUnit,hexGrid);
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
    [Command]
    public void CMDHideRange(Movement selectedUnit)
    {
        RPCHideRange(selectedUnit);
    }
    [ClientRpc]
    public void RPCHideRange(Movement selectedUnit)
    {
        // if(UnitManager.Instance.selectedUnit != selectable) return;
        if(movementRange.allNodesDict ==null) return;
        HideRange(selectedUnit);
    }
    
    
    // [Command]
    // public void CMDShowRange(Movement selectedUnit)
    // {
    //     RPCShowRange(selectedUnit);
    // }

    // [ClientRpc]
    // public void RPCShowRange(Movement selectedUnit)
    // {
    //     // if(UnitManager.Instance.selectedUnit != selectable) return;
    //     HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
    //     ShowRange(selectedUnit,);
    // }
    
    public abstract void CalculateRange(Movement selectedUnit,HexGrid hexGrid);
        
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


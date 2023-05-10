using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementSystem : Singleton<MovementSystem>
{
    public BFSResult movementRange = new BFSResult();
    private List<Vector3Int> currentPath = new List<Vector3Int>();
    public void HideRange(HexGrid hexGrid)
    {
        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            hexGrid.GetTileAt(hexPosition).DisableHighligh();
        }
        foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
        {
            hexGrid.GetTileAt(hexPosition).DisableHighligh(true);
        }
        movementRange = new BFSResult();
    }
    public void ShowRange(Unit selectedUnit,HexGrid hexGrid)
    {
        CalculateRange(selectedUnit,hexGrid);
        Vector3Int unitPos = hexGrid.GetClosestHex(selectedUnit.transform.position);
        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            if(unitPos == hexPosition) continue;
            hexGrid.GetTileAt(hexPosition).EnableHighligh();
        }
        foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
        {
            // hexGrid.GetTileAt(hexPosition).
            hexGrid.GetTileAt(hexPosition).EnableHighligh(true);
        }
        
    }
    public void CalculateRange(Unit selectedUnit,HexGrid hexGrid)
    {
        movementRange = GraphSearch.BsfGetRange(hexGrid,hexGrid.GetClosestHex(selectedUnit.transform.position),selectedUnit.MovementPoints);

    }
    public void ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid)
    {

        Hex hex = hexGrid.GetTileAt(selectedHexPosition);
        // if(hex.Unit != null && hex.Unit.Side == Side.Enemy)
        // {
            if(movementRange.GetRangeAllPositions().Contains(selectedHexPosition))
            {
                foreach (Vector3Int hexPosition in currentPath)
                {
                    hexGrid.GetTileAt(hexPosition).ResetHighlight();
                }
                currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition);
                foreach (Vector3Int hexPosition in currentPath)
                {
                    Debug.Log(hexGrid.GetTileAt(hexPosition));
                    hexGrid.GetTileAt(hexPosition).HighlightPath();
                    
                }
            }
        // }

        // else
        // {
        //     if(movementRange.GetRangePositions().Contains(selectedHexPosition))
        //     {
        //         foreach (Vector3Int hexPosition in currentPath)
        //         {
        //             hexGrid.GetTileAt(hexPosition).ResetHighlight();
        //         }
        //         currentPath = movementRange.GetPathTo(selectedHexPosition);
        //         foreach (Vector3Int hexPosition in currentPath)
        //         {
        //             hexGrid.GetTileAt(hexPosition).HighlightPath();
                    
        //         }
        //     }
        // }
    }

    public void MoveUnit(Unit selectedUnit,HexGrid hexGrid, Hex hex)
    {
        selectedUnit.Hex = hex;
        Debug.Log("Moving unit " + selectedUnit.name);
        selectedUnit.MoveThroughPath(currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList());
    }
    public bool IsHexInRange(Vector3Int hexPosition)
    {
        return movementRange.IsHecPositionInRange(hexPosition);
    }
    
}


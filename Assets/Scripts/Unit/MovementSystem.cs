using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementSystem : Singleton<MovementSystem>
{
    public float h;
    public BFSResult movementRange = new BFSResult();
    [SerializeField] private List<Vector3Int> currentPath = new List<Vector3Int>();
    public void HideRange(HexGrid hexGrid)
    {
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
            hexGrid.GetTileAt(hexPosition).EnableHighlighEnemy();
        }
        
    }
    public void CalculateRange(Unit selectedUnit,HexGrid hexGrid)
    {
        movementRange = GraphSearch.BsfGetRange(hexGrid,hexGrid.GetClosestHex(selectedUnit.transform.position),selectedUnit.GetCurrentMovementPoints());
    }
    public void ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid, int range = 1)
    {
        Hex hex = hexGrid.GetTileAt(selectedHexPosition);
        if(hex.isVisible)
        {
            if(hex.Unit != null && hex.Unit.Side == Side.Enemy)
            {
                if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? enemyHex = null;
                    currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,range);
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).HighlightPath();                    
                    }
                    if(currentPath.Count == 0)
                    {
                        currentPath.Add((Vector3Int)enemyHex);
                        // hexGrid.GetTileAt((Vector3Int)enemyHex).HighlightPath();                    
                    }
                }
            }

            else
            {
                if(movementRange.GetRangePositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    currentPath = movementRange.GetPathTo(selectedHexPosition);
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).HighlightPath();
                    }
                }
            }
        }
        else
        {
                if(movementRange.GetRangePositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    currentPath = movementRange.GetPathTo(selectedHexPosition);
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).HighlightPath();
                    }
                }
        }
    }
    private void OnDrawGizmos() {
        if(movementRange.allNodesDict== null)
        {
return;
        }
        foreach (var item in movementRange.allNodesDict )
        {
            Vector3 startPos = HexGrid.Instance.GetTileAt (item.Key).transform.position;
            if( item.Value != null)
            {
                Vector3 valuePos = HexGrid.Instance.GetTileAt ((Vector3Int)item.Value).transform.position;
                DrawArrow.ForGizmo(valuePos + Vector3.up * h,(startPos-valuePos),Color.black,.5f,25);
            }
        }
    }
    public void MoveUnit(Unit selectedUnit,HexGrid hexGrid, Hex hex,int range = 1)
    {
        if(selectedUnit.GetCurrentMovementPoints() == 0) 
            return;
        // selectedUnit.SetCurrentMovementPoints(selectedUnit.GetCurrentMovementPoints() - movementRange.GetCost(hex.HexCoordinates));
        
        List<Vector3> currentPathTemp = currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList(); 
        List<Hex> currentHexes = currentPath.Select(pos => hexGrid.GetTileAt(pos)).ToList(); 
            if(hex.IsEnemy())
            {
                if(currentPath.Count == 1 && hexGrid.GetTileAt (currentPath[0]).Unit != null && hexGrid.GetTileAt (currentPath[0]).Unit.Side == Side.Enemy)
                {
                    selectedUnit.MoveThroughPath(currentPathTemp,currentHexes , hex,false);
                }
                else
                {
                    selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex);
                }

            }
            else
            {
                selectedUnit.MoveThroughPath(currentPathTemp,currentHexes ,hex);
            }
    }
    public bool IsHexInRange(Vector3Int hexPosition)
    {
        return movementRange.IsHecPositionInRange(hexPosition);
    }
    
}


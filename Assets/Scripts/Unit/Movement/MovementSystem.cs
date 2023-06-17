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
    public MovementSystem()
    {
        
    }
    public void HideRange()
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
    
    public void RPCHideRange(Movement movement)
    {
        //  if(!UnitManager.Instance.selectedUnit == movement.GetComponent<Unit>())
        // {
        //     return;
        // }
       HideRange();
    }

    public void ShowRange(Movement selectedUnit)
    {
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
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
    
    public void RPCShowRange(Movement selectedUnit, Movement movement)
    {
        // if(!UnitManager.Instance.selectedUnit == movement.GetComponent<Unit>())
        // {
        //     return;
        // }
        ShowRange(selectedUnit);
        
    }
    public abstract void CalculateRange(Movement selectedUnit,HexGrid hexGrid);
        // if(selectedUnit.TryGetComponent(out Settler settler))
        // {
        //     movementRange = GraphSearch.BfsSettlerRange(hexGrid,hexGrid.GetClosestHex(selectedUnit.transform.position),selectedUnit.GetCurrentMovementPoints());
        // }
        // else
        // {
        //     movementRange = GraphSearch.BsfGetRange(hexGrid,hexGrid.GetClosestHex(selectedUnit.transform.position),selectedUnit.GetCurrentMovementPoints());
        // }
    public abstract void ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid);
    // {
    //     Hex hex = hexGrid.GetTileAt(selectedHexPosition);
    //     if(hex.isVisible)
    //     {
    //         if(hex.Unit != null && hex.Unit.Side == Side.Enemy)
    //         {
    //             if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
    //             {
    //                 foreach (Vector3Int hexPosition in currentPath)
    //                 {
    //                     hexGrid.GetTileAt(hexPosition).ResetHighlight();
    //                 }
    //                 Vector3Int? enemyHex = null;
    //                 currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid,range);
    //                 foreach (Vector3Int hexPosition in currentPath)
    //                 {
    //                     hexGrid.GetTileAt(hexPosition).HighlightPath();                    
    //                 }
    //                 if(currentPath.Count == 0)
    //                 {
    //                     currentPath.Add((Vector3Int)enemyHex);
    //                     // hexGrid.GetTileAt((Vector3Int)enemyHex).HighlightPath();                    
    //                 }
    //             }
    //         }
    //         else if(hex.Unit != null && hex.Unit.Side == Side.Me)
    //         {
    //             if(movementRange.GetRangeMePositions().Contains(selectedHexPosition))
    //             {
    //                 foreach (Vector3Int hexPosition in currentPath)
    //                 {
    //                     hexGrid.GetTileAt(hexPosition).ResetHighlight();
    //                 }
    //                 Vector3Int? meHex = null;
    //                 currentPath = movementRange.GetPathMeGrid(selectedHexPosition,out meHex,hexGrid,range);
    //                 foreach (Vector3Int hexPosition in currentPath)
    //                 {
    //                     hexGrid.GetTileAt(hexPosition).HighlightPath();
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             if(movementRange.GetRangePositions().Contains(selectedHexPosition))
    //             {
    //                 foreach (Vector3Int hexPosition in currentPath)
    //                 {
    //                     hexGrid.GetTileAt(hexPosition).ResetHighlight();
    //                 }
    //                 currentPath = movementRange.GetPathTo(selectedHexPosition);
    //                 foreach (Vector3Int hexPosition in currentPath)
    //                 {
    //                     hexGrid.GetTileAt(hexPosition).HighlightPath();
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         if(movementRange.GetRangePositions().Contains(selectedHexPosition))
    //         {
    //             foreach (Vector3Int hexPosition in currentPath)
    //             {
    //                 hexGrid.GetTileAt(hexPosition).ResetHighlight();
    //             }
    //             currentPath = movementRange.GetPathTo(selectedHexPosition);
    //             foreach (Vector3Int hexPosition in currentPath)
    //             {
    //                 hexGrid.GetTileAt(hexPosition).HighlightPath();
    //             }
    //         }
    //     }
    // }
    // private void OnDrawGizmos() {
    //     if(movementRange.allNodesDict2== null)
    //     {
    //         return;
    //     }
    //     foreach (var item in movementRange.allNodesDict2 )
    //     {
    //         Vector3 startPos = hexGrid.GetTileAt (item.Key).transform.position;
    //         if( item.Value != null)
    //         {
    //             Vector3 valuePos = hexGrid.GetTileAt ((Vector3Int)item.Value).transform.position;
    //             DrawArrow.ForGizmo(valuePos + Vector3.up * h,(startPos-valuePos),Color.black,.5f,25);
    //         }
    //     }
    // }
    public abstract void MoveUnit(Movement selectedUnit,HexGrid hexGrid, Hex hex,int range = 1);
    // {
    //     if(selectedUnit.GetCurrentMovementPoints() == 0) 
    //         return;
    //     // selectedUnit.SetCurrentMovementPoints(selectedUnit.GetCurrentMovementPoints() - movementRange.GetCost(hex.HexCoordinates));
        
    //     List<Vector3> currentPathTemp = currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList(); 
    //     List<Hex> currentHexes = currentPath.Select(pos => hexGrid.GetTileAt(pos)).ToList(); 
        
    //     if(hex.IsEnemy())
    //     {
    //         if(currentPath.Count == 0 && hexGrid.GetTileAt (currentPath[0]).Unit != null && hexGrid.GetTileAt (currentPath[0]).Unit.Side == Side.Enemy)
    //         {
    //             selectedUnit.GetComponent<Movement>().MoveThroughPath(currentPathTemp,currentHexes , hex,false);
    //         }
            
    //         else
    //         {
    //             selectedUnit.GetComponent<Movement>().MoveThroughPath(currentPathTemp,currentHexes, hex);
    //         }

    //     }
    //     else if(hex.IsMe())
    //     {
    //         if(currentPath.Count == 0 && hex.Unit != null && hex.Unit.Side == Side.Me)
    //         {
    //             selectedUnit.GetComponent<Movement>().ChangeHex(selectedUnit.GetComponent<Movement>(),hex.Unit.GetComponent<Movement>());
    //             return;
    //         }
    //         else
    //         {
    //             selectedUnit.GetComponent<Movement>().MoveThroughPath(currentPathTemp,currentHexes, hex);
    //         }
    //     }
    //     else
    //     {
    //         selectedUnit.GetComponent<Movement>().MoveThroughPath(currentPathTemp,currentHexes ,hex);
    //     }
    // }
    public bool IsHexInRange(Vector3Int start,Vector3Int hexPosition,int movementPoints)
    {
        BFSResult result = GraphSearch.BsfGetRange(hexGrid,start,movementPoints);

        return result.IsHecPositionInRange(hexPosition);
    }
    public bool IsHexInRange(Vector3Int hexPosition)
    {
        return movementRange.IsHecPositionInRange(hexPosition);
    }
    
    
}


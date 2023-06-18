using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlerMovementSystem : MovementSystem
{
    public SettlerMovementSystem(Movement movement) : base(movement)
    {
    }

    public override void CalculateRange(Movement selectedUnit, HexGrid hexGrid)
    {
        movementRange = GraphSearch.BfsSettlerRange(hexGrid,hexGrid.GetClosestHex(selectedUnit.transform.position),selectedUnit.GetCurrentMovementPoints());
    }
    public override void ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid)
    {
        Hex hex = hexGrid.GetTileAt(selectedHexPosition);
        if(hex.isVisible)
        {
            if((hex.Unit != null && hex.Unit.Side == Side.Enemy) || (hex.Settler != null && hex.Settler.side == Side.Enemy))
            {
                if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? enemyHex = null;
                    currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid);
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
            else if(hex.Settler != null && hex.Settler.side == Side.Me)
            {
                if(movementRange.GetRangeMePositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? meHex = null;
                    currentPath = movementRange.GetPathMeGrid(selectedHexPosition,out meHex,hexGrid);
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).HighlightPath();
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
    public override void MoveUnit(Movement selectedUnit,HexGrid hexGrid, Hex hex)
    {
         if(selectedUnit.GetCurrentMovementPoints() == 0) 
            return;
        // selectedUnit.SetCurrentMovementPoints(selectedUnit.GetCurrentMovementPoints() - movementRange.GetCost(hex.HexCoordinates));
        
        List<Vector3> currentPathTemp = currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList(); 
        List<Hex> currentHexes = currentPath.Select(pos => hexGrid.GetTileAt(pos)).ToList(); 
        
        // if(hex.IsEnemy() && hex.IsEnemySettler())
        // {
        //     if(currentPath.Count == 0 && (hexGrid.GetTileAt (currentPath[0]).Settler != null && hexGrid.GetTileAt (currentPath[0]).Settler.side == Side.Enemy)  || (hexGrid.GetTileAt (currentPath[0]).Unit != null && hexGrid.GetTileAt (currentPath[0]).Unit.Side == Side.Enemy))
        //     {
        //         selectedUnit.MoveThroughPath(currentPathTemp,currentHexes , hex,this,false);
        //     }
            
        //     else
        //     {
        //         selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);
        //     }

        // }
        // else if(hex.IsMe())
        // {
        //     if(currentPath.Count == 0 && hex.Settler != null && hex.Settler.side == Side.Me)
        //     {
        //         selectedUnit.ChangeHex(selectedUnit,hex.Settler.GetComponent<Movement>(),this);
        //         return;
        //     }
        //     else
        //     {
        //         selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);
        //     }
        // }
        // else
        // {
        //     selectedUnit.MoveThroughPath(currentPathTemp,currentHexes ,hex,this);
        // }
    }
}

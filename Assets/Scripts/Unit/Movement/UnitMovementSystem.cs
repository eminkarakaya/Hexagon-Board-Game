using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMovementSystem : MovementSystem
{
    public UnitMovementSystem(IMovable movement) : base(movement)
    {
        // CalculateRange(movement,hexGrid);
    }

    public override void CalculateRange(IMovable selectedUnit,HexGrid hexGrid)
    {
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        movementRange = GraphSearch.BsfGetRange(hexGrid,hexGrid.GetClosestHex(selectedUnit.Movement.transform.position),selectedUnit.Movement.GetCurrentMovementPoints());
    }
    public override void ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid)
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
            else if(hex.Unit != null && hex.Unit.Side == Side.Me)
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
    private void OnDrawGizmos() {
        if(movementRange.allNodesDict2== null)
        {
            return;
        }
        foreach (var item in movementRange.allNodesDict2 )
        {
            Vector3 startPos = hexGrid.GetTileAt (item.Key).transform.position;
            if( item.Value != null)
            {
                Vector3 valuePos = hexGrid.GetTileAt ((Vector3Int)item.Value).transform.position;
                DrawArrow.ForGizmo(valuePos + Vector3.up * h,(startPos-valuePos),Color.black,.5f,25);
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
        
        if(hex.IsEnemy())
        {
            if(currentPath.Count == 0 && hexGrid.GetTileAt (currentPath[0]).Unit != null && hexGrid.GetTileAt (currentPath[0]).Unit.Side == Side.Enemy)
            {
                selectedUnit.MoveThroughPath(currentPathTemp,currentHexes , hex,this,false);
            }
            
            else
            {
                selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);
            }

        }
        else if(hex.IsMe())
        {
            if(currentPath.Count == 0 && hex.Unit != null && hex.Unit.Side == Side.Me)
            {
                selectedUnit.ChangeHex(selectedUnit,hex.Unit.GetComponent<Movement>(),this);
                return;
            }
            else
            {
                selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);
            }
        }
        else
        {
            selectedUnit.MoveThroughPath(currentPathTemp,currentHexes ,hex,this);
        }
    }
    
    
}

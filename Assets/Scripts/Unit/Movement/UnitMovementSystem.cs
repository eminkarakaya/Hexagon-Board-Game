using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class UnitMovementSystem : MovementSystem
{
    public UnitMovementSystem(IMovable movement) : base(movement)
    {
        CalculateRange(movement,hexGrid);
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
            else if(hex.Building != null && hex.Building.Side == Side.Me)
            {
                if(movementRange.GetRangeAllPositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    currentPath = movementRange.GetPathBuildingGrid(selectedHexPosition,hexGrid);
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).HighlightPath();
                    }
                }
            }
            else if(hex.Building != null && hex.Building.Side == Side.Enemy)
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
        if(hex.IsEnemy() || hex.IsEnemyBuilding())
        {
            if(currentPath.Count == 0)
            {
                // selectedUnit.MoveThroughPath(currentPathTemp,currentHexes , hex,this,false);
                selectedUnit.StartCoroutineRotationUnit(selectedUnit,hex.transform.position,hex);
            }
            
            else
            {
                selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);
            }

        }
        else if(hex.IsEnemySettler())
        {
            
            selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);   
            selectedUnit.GetComponent<Unit>().CivManager.Capture(hex.Settler.GetComponent<NetworkIdentity>());     
            hex.Settler.StartCoroutine1(hex.Settler.GetComponent<NetworkIdentity>(),hex.Settler.gameObject);
            // selectedUnit.GetComponent<Unit>().Capture(hex.Settler.GetComponent<NetworkIdentity>(),hex.Settler.gameObject);
        }
        else if(hex.IsMe())
        {
            
            if(currentPath.Count == 0)
            {
                selectedUnit.ChangeHex(selectedUnit,hex.Unit.GetComponent<Movement>(),this);
                return;
            }
            else if(currentPath.Count == 0 && hex.Unit != null && hex.Unit.Side == Side.Me)
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

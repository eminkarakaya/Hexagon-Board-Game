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
    
    public override void ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid,int range)
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
                    currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid,range);
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
                    currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid,range);
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).HighlightPath();
                    }
                    
                }
            }
            else if(hex.Settler != null && hex.Settler.Side == Side.Enemy)
            {
                
                if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? enemyHex = null;
                    currentPath = movementRange.GetPathEnemyGridSettler(selectedHexPosition,out enemyHex,hexGrid,range);
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
        
        List<Vector3> currentPathTemp = currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList(); 
        List<Hex> currentHexes = currentPath.Select(pos => hexGrid.GetTileAt(pos)).ToList(); 
        if(hex.IsEnemy() || hex.IsEnemyBuilding())
        {
            if(currentPath.Count == 0)
            {
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

    public override void ShowRange(IMovable selectedUnit, Movement unit)
    {
        if(UnitManager.Instance.selectedUnit  == null) return;
        if(UnitManager.Instance.selectedUnit != unit.GetComponent<ISelectable>()) return;
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        CalculateRange(selectedUnit,hexGrid);
        Vector3Int unitPos = hexGrid.GetClosestHex(selectedUnit.Movement.transform.position);
        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            if(unitPos == hexPosition) continue;
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hex.EnableHighligh();
            hex.isReachable = true;
        }
        
        foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hex.EnableHighlighEnemy();
            hex.isReachable = true;
        }
    }
}

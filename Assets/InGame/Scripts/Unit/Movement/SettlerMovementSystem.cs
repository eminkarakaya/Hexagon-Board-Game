using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlerMovementSystem : MovementSystem
{
    public SettlerMovementSystem(IMovable movement) : base(movement)
    {
        CalculateRange(movement,hexGrid);
    }

    public override void CalculateRange(IMovable selectedUnit, HexGrid hexGrid)
    {
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        movementRange = GraphSearch.BfsSettlerRangeLand(hexGrid,hexGrid.GetClosestHex(selectedUnit.Hex.transform.position),selectedUnit.Movement.GetCurrentMovementPoints());
    }
    public override List<Vector3Int> ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid,int range)
    {
        Hex hex = hexGrid.GetTileAt(selectedHexPosition);
        if(hex.isVisible)
        {
            if((hex.Unit != null && hex.Unit.Side == Side.Enemy) || (hex.Settler != null && hex.Settler.Side == Side.Enemy))
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
                    }
                }
                return currentPath;
            }
            else if(hex.Settler != null && hex.Settler.Side == Side.Me)
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
                return currentPath;
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
                return currentPath;
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
                return currentPath;
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
            return currentPath;
        }
    }
    public override void MoveUnit(Movement selectedUnit,HexGrid hexGrid, Hex hex)
    {
         if(selectedUnit.GetCurrentMovementPoints() == 0) 
            return;
        
        List<Vector3> currentPathTemp = currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList(); 
        List<Hex> currentHexes = currentPath.Select(pos => hexGrid.GetTileAt(pos)).ToList(); 
        if(hex.IsEnemy() && hex.IsEnemySettler())
        {
            if(currentPath.Count == 0 && (hexGrid.GetTileAt (currentPath[0]).Settler != null && hexGrid.GetTileAt (currentPath[0]).Settler.Side == Side.Enemy)  || (hexGrid.GetTileAt (currentPath[0]).Unit != null && hexGrid.GetTileAt (currentPath[0]).Unit.Side == Side.Enemy))
            {
                selectedUnit.MoveThroughPath(currentPathTemp,currentHexes , hex,this,false);
            }
            
            else
            {
                selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);
            }

        }
        else if(hex.IsMeSettler())
        {
            if(currentPath.Count == 0 && hex.Settler != null && hex.Settler.Side == Side.Me)
            {
                selectedUnit.ChangeHex(selectedUnit,hex.Settler.GetComponent<Movement>(),this);
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

    public override void ShowRange(IMovable selectedUnit, Movement unit)
    {
        if(UnitManager.Instance.selectedUnit  == null) return;
        if(UnitManager.Instance.selectedUnit != unit.GetComponent<ISelectable>()) return;
        HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
        CalculateRange(selectedUnit,hexGrid);
        Vector3Int unitPos = hexGrid.GetClosestHex(selectedUnit.Hex.transform.position);
        IEnumerable<Vector3Int> poses = movementRange.GetRangePositions();
        IEnumerable<Vector3Int> enemyPoses = movementRange.GetRangeEnemiesPositions();
        foreach (Vector3Int hexPosition in poses)
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            if(unitPos == hexPosition) continue;
            // hex.EnableHighligh();
            hex.isReachable = true;
        }
        
        foreach (Vector3Int hexPosition in enemyPoses)
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hex.EnableHighlighEnemy();
            hex.isReachable = true;
        }
        foreach (Vector3Int hexPosition in poses)
        {
            // if(unitPos == hexPosition) continue;
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hexGrid.DrawBorders(hex,unitPos);

        }
        foreach (Vector3Int hexPosition in enemyPoses)
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hexGrid.DrawBorders(hex,unitPos);
        }
        
    }
}
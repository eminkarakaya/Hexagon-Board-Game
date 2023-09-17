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
    
    public override List<Vector3Int> ShowPath(Vector3Int selectedHexPosition,HexGrid hexGrid,int attackRange)
    {

        // if(hexGrid.GetTileAt(selectedHexPosition).GetHexInAnimation())
        // {
        //     return null;
        // }
        currentPath.Clear();
        
        Hex hex = hexGrid.GetTileAt(selectedHexPosition);
        if(hex.isVisible)
        {
            if(hex.IsEnemy())
            {
                if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? enemyHex = null;
                    List<Vector3Int> tempCurrentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid,attackRange);
                    // currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid,attackRange);

                    foreach (Vector3Int hexPosition in tempCurrentPath)
                    {
                        Hex hex1 = hexGrid.GetTileAt(hexPosition);
                        hex1.HighlightPath();                    
                        if(hex1.IsEnemy() || hex1.IsEnemyBuilding() ||hex1.IsEnemyShip())
                        {
                            return currentPath;
                        }
                        else
                        {
                            currentPath.Add(hex1.HexCoordinates);
                        }
                    }
                    
                }
                foreach (var item in currentPath)
                {
                }
                return currentPath;
            }
            else if(hex.IsMe())
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
            else if(hex.IsMeBuilding())
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
                return currentPath;
            }
            else if(hex.IsEnemyBuilding())
            {
                if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? enemyHex = null;
                    currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid,attackRange);
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).HighlightPath();
                    }
                }
                    return currentPath;
            }
            else if(hex.IsEnemySettler())
            {
                if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? enemyHex = null;
                    List<Vector3Int> tempCurrentPath = movementRange.GetPathEnemyGridSettler(selectedHexPosition,out enemyHex,hexGrid,attackRange);
                    // currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid,attackRange);



                    //  



                    foreach (Vector3Int hexPosition in tempCurrentPath)
                    {
                        Hex hex1 = hexGrid.GetTileAt(hexPosition);
                        hex1.HighlightPath();                    
                        if(hex1.IsEnemySettler())
                        {
                            currentPath.Add(hex1.HexCoordinates);
                            return currentPath;
                        }
                        else
                        {
                            currentPath.Add(hex1.HexCoordinates);
                        }
                    }
                    
                }
                // if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
                // {
                //     foreach (Vector3Int hexPosition in currentPath)
                //     {
                //         hexGrid.GetTileAt(hexPosition).ResetHighlight();
                //     }
                //     Vector3Int? enemyHex = null;
                //     currentPath = movementRange.GetPathEnemyGridSettler(selectedHexPosition,out enemyHex,hexGrid,attackRange);
                //     foreach (Vector3Int hexPosition in currentPath)
                //     {
                //         hexGrid.GetTileAt(hexPosition).HighlightPath();
                //     }
                // }
                    return currentPath;
            }
            else if(hex.IsEnemyShip())
            {
                if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? enemyHex = null;
                    currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid,attackRange);
                    List<Vector3Int> tempCurrentPath =  movementRange.GetPathMeGrid(selectedHexPosition,out enemyHex,hexGrid);
                    
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).HighlightPath();
                    }
                }
                    return currentPath;
            }
            else if(hex.IsMeShip())
            {
                if(movementRange.GetRangeMePositions().Contains(selectedHexPosition))
                {
                    foreach (Vector3Int hexPosition in currentPath)
                    {
                        hexGrid.GetTileAt(hexPosition).ResetHighlight();
                    }
                    Vector3Int? meHex = null;
                    List<Vector3Int> tempCurrentPath =  movementRange.GetPathMeGrid(selectedHexPosition,out meHex,hexGrid);
                    foreach (Vector3Int hexPosition in tempCurrentPath)
                    {
                        Hex hex1 = hexGrid.GetTileAt(hexPosition);
                        hex1.HighlightPath();                    
                        if(hex1.IsEnemy() || hex1.IsEnemyBuilding() ||hex1.IsEnemyShip())
                        {
                            return currentPath;
                        }
                        else
                        {
                            currentPath.Add(hex1.HexCoordinates);
                        }
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
                    List<Vector3Int> tempCurrentPath =  movementRange.GetPathTo(selectedHexPosition);
                    // currentPath = movementRange.GetPathTo(selectedHexPosition);
                    foreach (Vector3Int hexPosition in tempCurrentPath)
                    {
                        Hex hex1 = hexGrid.GetTileAt(hexPosition);
                        hex1.HighlightPath();                    
                        if(hex1.IsEnemy() || hex1.IsEnemyBuilding() ||hex1.IsEnemyShip())
                        {
                            return currentPath;
                        }
                        else
                        {
                            currentPath.Add(hex1.HexCoordinates);
                        }
                    }
                }
                foreach (var item in currentPath)
                {
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
                List<Vector3Int> tempCurrentPath =  movementRange.GetPathTo(selectedHexPosition);
                // currentPath = movementRange.GetPathTo(selectedHexPosition);
                foreach (Vector3Int hexPosition in tempCurrentPath)
                {
                    Hex hex1 = hexGrid.GetTileAt(hexPosition);
                    hex1.HighlightPath();                    
                    if(hex1.IsEnemy() || hex1.IsEnemyBuilding() ||hex1.IsEnemyShip())
                    {
                        foreach (var item in currentPath)
                        {
                        }
                        return currentPath;
                    }
                    else
                    {
                        currentPath.Add(hex1.HexCoordinates);
                    }
                }
            }
            foreach (var item in currentPath)
            {
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
        if(hex.IsEnemy() || hex.IsEnemyBuilding() || hex.IsEnemyShip())
        {
            if(currentPath.Count == 0)
            {
                // selectedUnit.StartCoroutineRotationUnit(selectedUnit,hex.transform.position,hex);
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
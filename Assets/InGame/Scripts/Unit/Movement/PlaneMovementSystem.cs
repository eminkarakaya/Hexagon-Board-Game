// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class PlaneMovementSystem : MovementSystem
// {
//     public PlaneMovementSystem(IMovable movement) : base(movement)
//     {
//         CalculateRange(movement,hexGrid);
//     }

//     public override void CalculateRange(IMovable selectedUnit, HexGrid hexGrid)
//     {
//         hexGrid = GameObject.FindObjectOfType<HexGrid>();
//         movementRange = GraphSearch.BsfGetRangeAir(hexGrid,hexGrid.GetClosestHex(selectedUnit.Hex.transform.position),selectedUnit.Movement.GetCurrentMovementPoints());
//     }
//     public override List<Vector3Int> ShowPath(Movement selectedMovement,Vector3Int selectedHexPosition,HexGrid hexGrid,int range)
//     {
//         Hex hex = hexGrid.GetTileAt(selectedHexPosition);
//         if(hex.isVisible)
//         {
//             if((hex.Plane != null && hex.Plane.Side == Side.Enemy))
//             {
//                 if(movementRange.GetRangeEnemiesPositions().Contains(selectedHexPosition))
//                 {
//                     foreach (Vector3Int hexPosition in currentPath)
//                     {
//                         hexGrid.GetTileAt(hexPosition).ResetHighlight();
//                     }
//                     Vector3Int? enemyHex = null;
//                     currentPath = movementRange.GetPathEnemyGrid(selectedHexPosition,out enemyHex,hexGrid);
//                     foreach (Vector3Int hexPosition in currentPath)
//                     {
//                         hexGrid.GetTileAt(hexPosition).HighlightPath();                    
//                     }
//                     if(currentPath.Count == 0)
//                     {
//                         currentPath.Add((Vector3Int)enemyHex);
//                     }
//                 }
//                 return currentPath;
//             }
//             else if(hex.Plane != null && hex.Plane.Side == Side.Me)
//             {
//                 if(movementRange.GetRangeMePositions().Contains(selectedHexPosition))
//                 {
//                     foreach (Vector3Int hexPosition in currentPath)
//                     {
//                         hexGrid.GetTileAt(hexPosition).ResetHighlight();
//                     }
//                     Vector3Int? meHex = null;
//                     currentPath = movementRange.GetPathMeGrid(selectedHexPosition,out meHex,hexGrid);
//                     foreach (Vector3Int hexPosition in currentPath)
//                     {
//                         hexGrid.GetTileAt(hexPosition).HighlightPath();
//                     }
//                 }
//                 return currentPath;
//             }
        
//             else
//             {
//                 if(movementRange.GetRangePositions().Contains(selectedHexPosition))
//                 {
//                     foreach (Vector3Int hexPosition in currentPath)
//                     {
//                         hexGrid.GetTileAt(hexPosition).ResetHighlight();
//                     }
//                     currentPath = movementRange.GetPathTo(selectedHexPosition);
//                     foreach (Vector3Int hexPosition in currentPath)
//                     {
//                         hexGrid.GetTileAt(hexPosition).HighlightPath();
//                     }
//                 }
//                 return currentPath;
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
//             return currentPath;
//         }
//     }
//     public override void MoveUnit(Movement selectedUnit,HexGrid hexGrid, Hex hex)
//     {
//          if(selectedUnit.GetCurrentMovementPoints() == 0) 
//             return;
        
//         List<Vector3> currentPathTemp = currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList(); 
//         List<Hex> currentHexes = currentPath.Select(pos => hexGrid.GetTileAt(pos)).ToList(); 
//         if(hex.IsEnemyPlane())
//         {
//             if(currentPath.Count == 0 && (hexGrid.GetTileAt (currentPath[0]).Plane != null && hexGrid.GetTileAt (currentPath[0]).Plane.Side == Side.Enemy)  || (hexGrid.GetTileAt (currentPath[0]).Unit != null && hexGrid.GetTileAt (currentPath[0]).Unit.Side == Side.Enemy))
//             {
//                 selectedUnit.MoveThroughPath(currentPathTemp,currentHexes , hex,this,false);
//             }
            
//             else
//             {
//                 selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);
//             }

//         }
//         else if(hex.IsMePlane())
//         {
//             if(currentPath.Count == 0 && hex.Plane != null && hex.Plane.Side == Side.Me)
//             {
//                 selectedUnit.ChangeHex(selectedUnit,hex.Plane.GetComponent<Movement>(),this);
//                 return;
//             }
//             else
//             {
//                 selectedUnit.MoveThroughPath(currentPathTemp,currentHexes, hex,this);
//             }
//         }
//         else
//         {
//             selectedUnit.MoveThroughPath(currentPathTemp,currentHexes ,hex,this);
//         }
//     }

//     public override void ShowRange(IMovable selectedUnit, Movement unit)
//     {
//         if(UnitManager.Instance.selectedUnit == null) return;
//         if(UnitManager.Instance.selectedUnit != unit.GetComponent<ISelectable>()) return;

//         HexGrid hexGrid = GameObject.FindObjectOfType<HexGrid>();
//         CalculateRange(selectedUnit,hexGrid);
//         Vector3Int unitPos = hexGrid.GetClosestHex(selectedUnit.Movement.transform.position);
//         foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
//         {
//             if(unitPos == hexPosition) continue;
//             Hex hex = hexGrid.GetTileAt(hexPosition);
//             // hex.EnableHighligh();
//             hex.isReachable = true;
//         }
//         foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
//         {
//             Hex hex = hexGrid.GetTileAt(hexPosition);
//             hex.EnableHighlighEnemy();
//             hex.isReachable = true;
//         }
//     }
// }

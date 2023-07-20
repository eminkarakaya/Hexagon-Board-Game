using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipMovementSystem : UnitMovementSystem
{
    public ShipMovementSystem(IMovable movement) : base(movement)
    {
        CalculateRange(movement,hexGrid);
    }
    public override void CalculateRange(IMovable selectedUnit,HexGrid hexGrid)
    {
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        movementRange = GraphSearch.BsfGetRangeSea(hexGrid,hexGrid.GetClosestHex(selectedUnit.Movement.transform.position),selectedUnit.Movement.GetCurrentMovementPoints());
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
        else if(hex.IsMeShip())
        {
            
            if(currentPath.Count == 0)
            {
                selectedUnit.ChangeHex(selectedUnit,hex.Ship.GetComponent<Movement>(),this);
                return;
            }
            else if(currentPath.Count == 0 && hex.Ship != null && hex.Ship.Side == Side.Me)
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

using System.Collections;
using System.Collections.Generic;
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
  
}

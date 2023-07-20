using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlerMovementSystemSeaAndLand : SettlerMovementSystem
{
    public SettlerMovementSystemSeaAndLand(IMovable movement) :  base(movement)
    {
        CalculateRange(movement,hexGrid);
    }

    public override void CalculateRange(IMovable selectedUnit, HexGrid hexGrid)
    {
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        movementRange = GraphSearch.BfsSettlerRangeSeaAndLand(hexGrid,hexGrid.GetClosestHex(selectedUnit.Hex.transform.position),selectedUnit.Movement.GetCurrentMovementPoints());
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
            // hex.EnableHighligh();
            hex.isReachable = true;
        }
        foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hex.EnableHighlighEnemy();
            hex.isReachable = true;
        }
        foreach (Vector3Int hexPosition in movementRange.GetRangePositions())
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hexGrid.DrawBorders(hex,unitPos);
        }
        foreach (Vector3Int hexPosition in movementRange.GetRangeEnemiesPositions())
        {
            Hex hex = hexGrid.GetTileAt(hexPosition);
            hexGrid.DrawBorders(hex,unitPos);
        }
        
        
    }
}

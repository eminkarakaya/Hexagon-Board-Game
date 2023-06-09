using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem 
{
    [SerializeField] private HexGrid hexGrid;
    BFSResult rangeInfo;
    public void GetRange(IAttackable selectedUnit)
    {
        if(selectedUnit.Attack == null)
            return;
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        rangeInfo = GraphSearch.GetRange(hexGrid,selectedUnit.Hex.HexCoordinates,selectedUnit.Attack.range);
    }
    public void ShowRange(IAttackable selectedUnit)
    {
        if(!selectedUnit.Attack.TryGetComponent(out Range range))
        {
            return;
        }
        // if(selectedUnit.GetComponent<Movement>().GetCurrentMovementPoints() == 0) return;
        GetRange(selectedUnit);
        foreach (var item in rangeInfo.rangeNodesDict)
        {
            hexGrid.GetTileAt(item.Key).EnableHighlighRange();
        }
    }
    public void HideRange(IAttackable attackable)
    {
        if(!attackable.Attack.TryGetComponent(out Range range)) return;
        foreach (var item in rangeInfo.rangeNodesDict)
        {
            hexGrid.GetTileAt(item.Key).DisableHighlighRange();
        }
    }
    public bool CheckEnemyInRange(Hex enemyHex)
    {
        return rangeInfo.rangeNodesDict.ContainsKey(enemyHex.HexCoordinates);
    }
}
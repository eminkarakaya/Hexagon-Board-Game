using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem 
{
    public AttackSystem(IAttackable attackable)
    {
        GetRange(attackable);
    }
    [SerializeField] private HexGrid hexGrid;
    BFSResult rangeInfo;
    public void GetRange(IAttackable selectedUnit)
    {
        if(selectedUnit.Attack == null)
            return;
        hexGrid = GameObject.FindObjectOfType<HexGrid>();
        rangeInfo = GraphSearch.GetRange(hexGrid,selectedUnit.Hex.HexCoordinates,selectedUnit.Attack.range);
    }
    public int ShowRange(IAttackable selectedUnit)
    {
        if(!selectedUnit.Attack.TryGetComponent(out Range range))
        {
            return 0;
        }
        // if(selectedUnit.GetComponent<Movement>().GetCurrentMovementPoints() == 0) return;
        GetRange(selectedUnit);
        foreach (var item in rangeInfo.rangeNodesDict)
        {
            hexGrid.GetTileAt(item.Key).EnableHighlighRange();
        }
        return selectedUnit.Attack.range;
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
        Debug.Log(rangeInfo+ "rangeInfo");
        return rangeInfo.rangeNodesDict.ContainsKey(enemyHex.HexCoordinates);
    }
}
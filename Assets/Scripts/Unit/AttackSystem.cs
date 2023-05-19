using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    BFSResult rangeInfo;
    public void GetRange(Unit selectedUnit)
    {
        if(!selectedUnit.TryGetComponent(out Attack range))
            return;
        range = selectedUnit.GetComponent<Attack>();
        rangeInfo = GraphSearch.GetRange(HexGrid.Instance,selectedUnit.Hex.HexCoordinates,range.range);
    }
    public void ShowRange(Unit selectedUnit)
    {
        if(selectedUnit.GetCurrentMovementPoints() == 0) return;
        GetRange(selectedUnit);
        foreach (var item in rangeInfo.rangeNodesDict)
        {
            HexGrid.Instance.GetTileAt(item.Key).EnableHighlighRange();
        }
        
        
    }
    public void HideRange()
    {
        foreach (var item in rangeInfo.rangeNodesDict)
        {
            HexGrid.Instance.GetTileAt(item.Key).DisableHighlighRange();
        }
    }
    public bool CheckEnemyInRange(Hex enemyHex)
    {
        return rangeInfo.rangeNodesDict.ContainsKey(enemyHex.HexCoordinates);
    }
}

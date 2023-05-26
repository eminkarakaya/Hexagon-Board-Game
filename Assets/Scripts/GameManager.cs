using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public List<Unit> liveUnits;
    public Side side;
    
    public void SightAllUnits()
    {
        HexGrid.Instance.CloseVisible();
        foreach (var item in liveUnits)
        {
            item.ShowSight(item.Hex);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BuildingHP : HP
{
    public override void Death()
    {
        // manager.Capture(identity);
    }
    public override void Death(NetworkIdentity identity)
    {
        manager.Capture(identity);
        TeamColor [] teamColors = GetComponent<Building>().transform.GetComponentsInChildren<TeamColor>();
        foreach (var item in teamColors)
        {
            item.SetColor(manager.data);
        }
    }
   
}

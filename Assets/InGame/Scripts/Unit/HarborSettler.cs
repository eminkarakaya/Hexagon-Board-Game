using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class HarborSettler : Settler
{
    [Command] public override void CMDCreateBuilding()
    {
        if(Hex.Building != null) return;
        if(!Hex.isCoast) return;
        Building unit = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateBuilding(unit);
    }
}

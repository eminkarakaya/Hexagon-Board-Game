using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
public class PlayerManager : CivManager
{
    public List<IMovable> liveUnits;
   
    private void Start() {
        if(!isLocalPlayer && !isOwned)
            return;
        CMDCreateBuilding();
    }
   
    public static PlayerManager FindPlayerManager()
    {
        var managers = FindObjectsOfType<PlayerManager>();
        foreach (var item in managers)
        {
            if(item.isOwned)
                return item;
        }
        return null;
    }
    
    
    
    [Command] // client -> server
    private void CMDCreateBuilding()
    {
        Building unit = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        
        ownedObjs.Add(unit.gameObject);
        RPCCreateBuilding(unit,NetworkManagerGdd.singleton.playersHexes.Count-1);
        
        FindPlayerManager(unit);
    }
    
    [ClientRpc] // server -> client
    private void RPCCreateBuilding(Building unit,int i)
    {
        
        NetworkManagerGdd.singleton.buildings = FindObjectsOfType<Building>().ToList();
        unit.transform.position = new Vector3 (NetworkManagerGdd.singleton.playersHexes[i]. transform.position.x , 1 , NetworkManagerGdd.singleton.playersHexes[i]. transform.position.z );
        unit.transform.rotation = Quaternion.Euler(-90,0,0); 
        unit.Hex = NetworkManagerGdd.singleton.playersHexes[i];
        unit.Hex.Building = unit;
        
        foreach (var item in NetworkManagerGdd.singleton.buildings)
        {
            if(item == null) continue;
            if(item.isOwned)
            {
                item.SetSide(Side.Me,item.GetComponent<Outline>());
            }
            else
                item.SetSide(Side.Enemy,item.GetComponent<Outline>());  
        }
        // Debug.Log(unit,unit);
        var managers = FindObjectsOfType<PlayerManager>();
       
        NetworkManagerGdd.singleton.playersHexes.RemoveAt(NetworkManagerGdd.singleton.playersHexes.Count-1);
    }

    [ClientRpc] private void FindPlayerManager(Building unit)
    {
       
        unit.CivManager = this;
        StartCoroutine(FindPlayerManagerIE(unit));
    }
    private IEnumerator FindPlayerManagerIE(Building unit)
    {
        while(unit.CivManager == null)
        {
            yield return null;
        }
        unit.CivManager.SetTeamColor(this.gameObject);

    }
        
    [Command]
    public override void CMDHideAllUnits()
    {
        RPCHideAllUnits();
    }
   
    [Command]
    public override void CMDShowAllUnits()
    {
        RPCShowAllUnits();
    }
    
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
public class PlayerManager : CivManager
{
    public List<IMovable> liveUnits;
   
    private void Start() {
        
            // PlayerManager[] objs = FindObjectsOfType<PlayerManager>();
            //     for (int i = 0; i < objs.Length; i++)
            //     {
            //         if(objs[i].isOwned == false)
            //         {
            //             Destroy(objs[i]);
            //         }
            //         else
            //         {
            //             DontDestroyOnLoad(objs[i]);

            //         }
            //     }

            if(!isLocalPlayer)
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
        
        AssignBuildings();
        Building unit = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        ownedObjs.Add(unit.gameObject);
        RPCCreateBuilding(unit,NetworkManagerGdd.singleton.playersHexes.Count-1);
    }
    
    [ClientRpc] private void AssignBuildings()
    {
        
        NetworkManagerGdd.singleton.buildings = FindObjectsOfType<Building>().ToList();
    }
    [ClientRpc] // server -> client
    private void RPCCreateBuilding(Building unit,int i)
    {
        
        unit.transform.position = new Vector3 (NetworkManagerGdd.singleton.playersHexes[i]. transform.position.x , 1 , NetworkManagerGdd.singleton.playersHexes[i]. transform.position.z );
        unit.transform.rotation = Quaternion.Euler(-90,0,0); 
        unit.Hex = NetworkManagerGdd.singleton.playersHexes[i];
        unit.Hex.Building = unit;
        foreach (var item in NetworkManagerGdd.singleton.buildings)
        {
            if(item == null) continue;
            if(item.isOwned)
            {
                item.GetComponent<ISelectable>().SetSide(Side.Me,item.GetComponent<Outline>());
            }
            else
                item.GetComponent<ISelectable>().SetSide(Side.Enemy,item.GetComponent<Outline>());  
        }
        NetworkManagerGdd.singleton.playersHexes.RemoveAt(NetworkManagerGdd.singleton.playersHexes.Count-1);
    }
    

        
    [Command]
    public override void CMDHideAllUnits()
    {
        HideAllUnits();
    }
   
    [Command]
    public override void CMDShowAllUnits()
    {
        ShowAllUnits();
    }
    
    
}

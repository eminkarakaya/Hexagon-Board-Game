using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class AIManager : CivManager
{
    
    public override void OnStartServer()
    {
        CMDCreateBuilding();
    }
    private void Start() {
        // GetComponent<NetworkIdentity>().RemoveClientAuthority();
        // GetComponent<NetworkIdentity>().AssignClientAuthority(NetworkManagerGdd.singleton.connection);
    }
    // client -> server
    [Server]
    private void CMDCreateBuilding()
    {
        
        // StartCoroutine(waitServerActive());
        AssignBuildings();
        Building unit = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        ownedObjs.Add(unit.gameObject);
        RPCCreateBuilding(unit,NetworkManagerGdd.singleton.npcHexes.Count-1);
        NetworkManagerGdd.singleton.npcHexes.RemoveAt(NetworkManagerGdd.singleton.npcHexes.Count-1);
    }
    [Server]
    private void AssignBuildings()
    {
        
        NetworkManagerGdd.singleton.buildings = FindObjectsOfType<Building>().ToList();
    }
    // server -> client
    
    private void RPCCreateBuilding(Building unit,int i)
    {
        unit.civManager = this;
        unit.transform.position = new Vector3 (NetworkManagerGdd.singleton.npcHexes[i]. transform.position.x , 1 , NetworkManagerGdd.singleton.npcHexes[i]. transform.position.z );
        unit.transform.rotation = Quaternion.Euler(-90,0,0); 
        unit.Hex = NetworkManagerGdd.singleton.npcHexes[i];
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
    }
    
}

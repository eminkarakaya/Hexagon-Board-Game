using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
public class PlayerManager : SingletonMirror<PlayerManager>
{
    [SerializeField] private GameObject buildingPrefab;
    public int team;
    public List<Unit> liveUnits;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // if(!isLocalPlayer)
            //             return;
            //     if(isServer)
            //     {
            //         Debug.Log(" server");
            //         // GetRandom();
            //         CreateBuilding();
            //     }
            //     else
            //     {
            //         Debug.Log(" client");
            //         // CmdGetRandom();   
            //         CMDCreateBuilding();
            //     }
        }
    }
    
    [Command]
    private void CreateBuilding()
    {
        GameManager.Instance.buildings.Add(null);
        // AssignBuildings();
        Building unit = Instantiate(GameManager.Instance.buildingGO[GameManager.Instance.buildings.Count-1],GameManager.Instance.hexes[GameManager.Instance.buildings.Count-1]. transform.position,Quaternion.identity).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateBuilding(unit);
        // NetworkServer.ReplacePlayerForConnection(connectionToClient,unit.gameObject);
        // // team = GameManager.Instance.buildings.Count-1;
        //  foreach (var item in GameManager.Instance.buildings)
        // {
        //     if(item.isOwned)
        //     {
        //         item.SetSide(Side.Me);
        //     }
        //     else
        //         item.SetSide(Side.Enemy);
            
        // }
    }
   
    
    [Command] // client -> server
    private void CMDCreateBuilding()
    {
        // GameManager.Instance.buildings.Add(null);
        AssignBuildings();
        Building unit = Instantiate(buildingPrefab,GameManager.Instance.hexes[GameManager.Instance.buildings.Count-1]. transform.position,Quaternion.identity).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);

        // team = GameManager.Instance.buildings.Count-1;
        RPCCreateBuilding(unit);
    }
    [ClientRpc] private void AssignBuildings()
    {
        GameManager.Instance.buildings = FindObjectsOfType<Building>().ToList();
    }
    [ClientRpc] // server -> client
    private void RPCCreateBuilding(Building unit)
    {

        unit.Hex = GameManager.Instance.hexes[GameManager.Instance.buildings.Count-1];
        unit.transform.position = unit.Hex.transform.position;
        // NetworkServer.ReplacePlayerForConnection(connectionToClient,unit.gameObject);
        // GameManager.Instance.buildings.Add(unit);
        foreach (var item in GameManager.Instance.buildings)
        {
            if(item == null) continue;
            // Debug.Log(item);
            if(item.isOwned)
            {
                item.SetSide(Side.Me);
            }
            else
                item.SetSide(Side.Enemy);  
        }
    }
    
    public override void OnStartClient()
    {
        // base.OnStartClient();
            if(!isLocalPlayer)
                    return;
            if(isServer)
            {
                Debug.Log(" server");
                // GetRandom();
                CreateBuilding();
            }
            else
            {
                Debug.Log(" client");
                // CmdGetRandom();   
                CMDCreateBuilding();
            }
    }
    public void SightAllUnits()
    {
        HexGrid.Instance.CloseVisible();
        foreach (var item in liveUnits)
        {
            item.HideSight(item.Hex);
        }
        foreach (var item in liveUnits)
        {
            item.ShowSight1(item.Hex);
        }
    }
    
    
}

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
        // GetComponent<NetworkIdentity>().AssignClientAuthority(GameManager.singleton.connection);
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
        RPCCreateBuilding(unit,GameManager.singleton.npcHexes.Count-1);
        GameManager.singleton.npcHexes.RemoveAt(GameManager.singleton.npcHexes.Count-1);
    }
    [Server]
    private void AssignBuildings()
    {
        
        GameManager.singleton.buildings = FindObjectsOfType<Building>().ToList();
    }
    // server -> client
    
    private void RPCCreateBuilding(Building unit,int i)
    {
        unit.CivManager = this;
        unit.transform.position = new Vector3 (GameManager.singleton.npcHexes[i]. transform.position.x , 1 , GameManager.singleton.npcHexes[i]. transform.position.z );
        unit.transform.rotation = Quaternion.Euler(-90,0,0); 
        unit.Hex = GameManager.singleton.npcHexes[i];
        unit.Hex.Building = unit;
        foreach (var item in GameManager.singleton.buildings)
        {
            if(item == null) continue;
            if(item.isOwned)
            {
                item.SetSide(Side.Me,item.GetComponent<Outline>());
            }
            else
                item.SetSide(Side.Enemy,item.GetComponent<Outline>());  
        }
    }

    public override void AddOrderList(ITaskable taskable)
    {

    }

    public override void RemoveOrderList(ITaskable taskable)
    {

    }

    public override void NextTurnBtn()
    {

    }

    public override void GetOrder()
    {

    }

    public override void GetOrderIcon()
    {

    }

    public override void ResetOrderIndex()
    {

    }

    public override void NextTurn()
    {

    }
}

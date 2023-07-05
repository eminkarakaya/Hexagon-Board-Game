using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;
using TMPro;

public class PlayerManager : CivManager
{
    public int team;
    public Side side;
    public List<IMovable> liveUnits;
    [SerializeField] private GameObject civUIPrefab;
   
    private void Start() {
        team = Random.Range(0,2);
        
            SetSide();
        if(isOwned)
        {
            CMDCreateCivUI();
            CMDCreateBuilding();
        }

    }
    
    public void SetSide()
    {
        side = Side.Me;
        var managers = FindObjectsOfType<PlayerManager>();
        foreach (var item in managers)
        {
            if(isOwned) continue;

            if(item.team == team)
            {
                item.side = Side.Ally;
            }
            else
            {
                item.side = Side.Enemy;
            }
        }
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

    
    [Command] private void CMDCreateCivUI()
    {
        CivDataUI civDataUI = Instantiate(civUIPrefab,NetworkManagerGdd.singleton.civUIParent).GetComponent<CivDataUI>();
        NetworkServer.Spawn(civDataUI.gameObject,connectionToClient);
        RPGCreateCivUI(civDataUI);
        
    }
    [ClientRpc] private void RPGCreateCivUI(CivDataUI civDataUI)
    {
        civDataUI.civManager = this;
        civDataUI.civData = civData;
        foreach (var item in FindObjectsOfType<CivDataUI>().ToList())
        {
            item.transform.SetParent(NetworkManagerGdd.singleton.civUIParent);
        }
    }
    
    [Command] // client -> server
    private void CMDCreateBuilding()
    {
        Building building = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(building.gameObject,connectionToClient);
        
        ownedObjs.Add(building.gameObject);
        RPCCreateBuilding(building,NetworkManagerGdd.singleton.playersHexes.Count-1);
        
        FindPlayerManager(building);
    }
    
    [ClientRpc] // server -> client
    private void RPCCreateBuilding(Building building,int i)
    {
        
        NetworkManagerGdd.singleton.buildings = FindObjectsOfType<Building>().ToList();
        building.transform.position = new Vector3 (NetworkManagerGdd.singleton.playersHexes[i]. transform.position.x , 1 , NetworkManagerGdd.singleton.playersHexes[i]. transform.position.z );
        building.transform.rotation = Quaternion.Euler(-90,0,0); 
        building.Hex = NetworkManagerGdd.singleton.playersHexes[i];
        building.Hex.Building = building;
        
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
        var managers = FindObjectsOfType<PlayerManager>();
       
        NetworkManagerGdd.singleton.playersHexes.RemoveAt(NetworkManagerGdd.singleton.playersHexes.Count-1);
        SetTeamColor(building.gameObject);
    }

    [ClientRpc] private void FindPlayerManager(Building building)
    {
       
        building.CivManager = this;
        StartCoroutine(FindPlayerManagerIE(building));
    }
    private IEnumerator FindPlayerManagerIE(Building building)
    {
        while(building.CivManager == null)
        {
            yield return null;
        }
        building.CivManager.SetTeamColor(this.gameObject);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private HexGrid hexGrid;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject buildingPrefab;
    public int team;
    public List<Unit> liveUnits;
    
    private void Start() {
        
        PlayerManager[] objs = FindObjectsOfType<PlayerManager>();
            for (int i = 0; i < objs.Length; i++)
            {
                if(objs[i].isOwned == false)
                {
                    Destroy(objs[i]);
                }
                else
                {
                    DontDestroyOnLoad(objs[i]);

                }
            }

            if(!isLocalPlayer)
                return;
            
            CMDCreateBuilding();
    }
    
    [Command] // client -> server
    private void CMDCreateBuilding()
    {
        gameManager = FindObjectOfType<GameManager>();
        int i = gameManager.buildings.Count-1;
        if(i == -1)
        {
            i = 0;
        }
        AssignBuildings();
        Building unit = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
    
        RPCCreateBuilding(unit);
    }
    
    [ClientRpc] private void AssignBuildings()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.buildings = FindObjectsOfType<Building>().ToList();
    }
    [ClientRpc] // server -> client
    private void RPCCreateBuilding(Building unit)
    {
        gameManager = FindObjectOfType<GameManager>();
        int i = gameManager.buildings.Count-1;
        if(i == -1)
        {
            i = 0;
        }
        unit.transform.position = new Vector3 (gameManager.hexes[i]. transform.position.x , 1 , gameManager.hexes[i]. transform.position.z );
        unit.transform.rotation = Quaternion.Euler(-90,0,0); 
        unit.Hex = gameManager.hexes[i];
        unit.Hex.Building = unit;
        foreach (var item in gameManager.buildings)
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
    

        
    [Command]
    public void CMDHideAllUnits()
    {
        HideAllUnits();
    }
   
    [Command]
    public void CMDShowAllUnits()
    {
        ShowAllUnits();
    }
    [ClientRpc] private void HideAllUnits()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        hexGrid.CloseVisible();
        List<Unit> allUnits = FindObjectsOfType<Unit>().ToList();
        foreach (var item in allUnits)
        {
            item.HideSight(item.Hex);
        }
    }
    [ClientRpc] private void ShowAllUnits()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        hexGrid.CloseVisible();
        List<Unit> allUnits = FindObjectsOfType<Unit>().ToList();
        foreach (var item in allUnits)
        {
            item.ShowSight(item.Hex);
        }
    }
}

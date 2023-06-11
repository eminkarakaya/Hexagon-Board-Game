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
    private void Awake() {
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
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
                if(isServer)
                {
                    CreateBuilding();
                }
                else
                {
                    CMDCreateBuilding(); 
                }
        }
    }
    
    [Command]
    private void CreateBuilding()
    {
        gameManager = FindObjectOfType<GameManager>();
        int i = gameManager.buildings.Count-1;
        if(i == -1)
        {
            i = 0;
        }
        Building unit = Instantiate(buildingPrefab,gameManager.hexes[i]. transform.position,Quaternion.identity).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateBuilding(unit);
      
    }
   
    
    [Command] // client -> server
    private void CMDCreateBuilding()
    {
        gameManager = FindObjectOfType<GameManager>();
        // gameManager.buildings.Add(null);
        int i = gameManager.buildings.Count-1;
        if(i == -1)
        {
            i = 0;
        }
        AssignBuildings();
        Building unit = Instantiate(buildingPrefab,gameManager.hexes[i]. transform.position,Quaternion.identity).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);

        // team = gameManager.buildings.Count-1;
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
        unit.Hex = gameManager.hexes[i];
        unit.Hex.Building = unit;
        unit.transform.position = unit.Hex.transform.position;
        // NetworkServer.ReplacePlayerForConnection(connectionToClient,unit.gameObject);
        // gameManager.buildings.Add(unit);
        foreach (var item in gameManager.buildings)
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
            // if(!isLocalPlayer)
            //         return;
            // if(isServer)
            // {
            //     Debug.Log(" server");
            //     // GetRandom();
            //     CreateBuilding();
            // }
            // else
            // {
            //     Debug.Log(" client");
            //     // CmdGetRandom();   
            //     CMDCreateBuilding();
            // }
    }
    public void SightAllUnits()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        hexGrid.CloseVisible();
        foreach (var item in liveUnits)
        {
            Debug.Log(item + " item");
            item.HideSight(item.Hex);
        }
        foreach (var item in liveUnits)
        {
            item.ShowSight1(item.Hex);
            Debug.Log(item + " item");
        }
    }
    
    
}

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Linq;
using Steamworks;


public class CivManager : NetworkBehaviour
{
    

    [SerializeField] protected GameObject buildingPrefab;
    [SerializeField] public List<GameObject> ownedObjs = new List<GameObject>();
    [SerializeField] private HexGrid hexGrid;
    public CivData civData;
    [Command]
    public void Capture(NetworkIdentity identity)
    {
        // NetworkServer.ReplacePlayerForConnection(connectionToClient,)
        identity.RemoveClientAuthority();
        identity.AssignClientAuthority(connectionToClient);
    }
    [Command] public void DestroyObj(GameObject obj)
    {
        Destroy(obj);
    }
    public void SetTeamColor(GameObject obj)
    {
        TeamColor [] teamColors = obj.GetComponentsInChildren<TeamColor>();
        foreach (var item in teamColors)
        {
            item.SetColor(civData);
        }

    }

    #region  Vision
    
    [Command]
    public virtual void CMDHideAllUnits()
    {
        RPCHideAllUnits();
    }
   
    [Command]
    public virtual void CMDShowAllUnits()
    {
        RPCShowAllUnits();
    }
    [ClientRpc] protected void RPCHideAllUnits()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        hexGrid.CloseVisible();
        List<Vision> allUnits = FindObjectsOfType<Vision>().ToList();
        foreach (var item in allUnits)
        {
            item.HideVision(item.GetComponent<IVisionable>().Hex);
        }
    }
    [ClientRpc] protected void RPCShowAllUnits()
    {
        
        hexGrid = FindObjectOfType<HexGrid>();
        hexGrid.CloseVisible();
        List<Vision> allUnits = FindObjectsOfType<Vision>().Where(x=>x.isOwned == true).ToList();
        foreach (var item in allUnits)
        {
            item.ShowVision(item.visionable.Hex);
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Linq;

public class CivManager : NetworkBehaviour
{
    [SerializeField] protected GameObject buildingPrefab;

    [SerializeField] public List<GameObject> ownedObjs = new List<GameObject>();
    [SerializeField] private HexGrid hexGrid;
    public CivData data;
    [Command()]
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
            item.SetColor(data);
        }

    }
    [Command]
    public virtual void CMDHideAllUnits()
    {
        HideAllUnits();
    }
   
    [Command]
    public virtual void CMDShowAllUnits()
    {
        ShowAllUnits();
    }
    [ClientRpc] protected void HideAllUnits()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        hexGrid.CloseVisible();
        List<Sight> allUnits = FindObjectsOfType<Sight>().ToList();
        foreach (var item in allUnits)
        {
            // Debug.Log(item + " a " + item.GetComponent<IMovable>() + " a  " + item.GetComponent<IMovable>().Hex,item.GetComponent<IMovable>().Movement);
            item.HideSight(item.GetComponent<ISightable>().Hex);
        }
    }
    [ClientRpc] protected void ShowAllUnits()
    {
        
        hexGrid = FindObjectOfType<HexGrid>();
        hexGrid.CloseVisible();
        List<Sight> allUnits = FindObjectsOfType<Sight>().Where(x=>x.isOwned == true).ToList();
        foreach (var item in allUnits)
        {
            item.ShowSight(item.sightable.Hex);
        }
    }
}

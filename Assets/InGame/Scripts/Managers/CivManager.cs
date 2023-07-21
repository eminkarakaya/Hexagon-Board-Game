using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Steamworks;


public abstract class CivManager : NetworkBehaviour
{
    protected Button orderButton;
    [SerializeField] protected Sprite nextTurnSprite,waitingSprite;    
    [SerializeField] protected List<ITaskable> orderList = new List<ITaskable>();
    [SerializeField] protected GameObject buildingPrefab;
    [SyncVar] [SerializeField] public List<GameObject> ownedObjs = new List<GameObject>();
    [SerializeField] private HexGrid hexGrid;
    public CivData civData;



    private void Start() {
        
    }
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


    [Command(requiresAuthority = false)] public void CMDAddOwnedObject(GameObject obj)
    {
        RPCAddOwnedObj(obj);
    }
    [ClientRpc] private void RPCAddOwnedObj(GameObject obj)
    {

        if(!ownedObjs.Contains(obj))
        {
            ownedObjs.Add(obj);
        }
    }
    [Command(requiresAuthority = false)] public void CMDRemoveOwnedObject(GameObject obj)
    {
        RPCRemoveOwnedObj(obj);
    }
    [ClientRpc] private void RPCRemoveOwnedObj(GameObject obj)
    {

        if(ownedObjs.Contains(obj))
        {
            ownedObjs.Remove(obj);
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

    public abstract void AddOrderList(ITaskable taskable);
    
    public abstract void RemoveOrderList(ITaskable taskable);

    public abstract void NextTurnBtn();
    public abstract void NextTurn();
    
    public abstract void GetOrder();
    
    public abstract void GetOrderIcon();
    
    

    public abstract void ResetOrderIndex();
    
}

using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Steamworks;


public class CivManager : NetworkBehaviour
{
    protected Button orderButton;
    [SerializeField] protected Sprite nextTurnSprite;    
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

    public void AddOrderList(ITaskable taskable)
    {
        if(!orderList.Contains(taskable))
        {
            orderList.Add(taskable);
        }
    }
    public void RemoveOrderList(ITaskable taskable)
    {
        if(orderList.Contains(taskable))
        {
            orderList.Remove(taskable);
        }

        GetOrderIcon();
    }

    public void NextTurn()
    {
        ResetOrderIndex();
        GetOrderIcon();
    }
    public void GetOrder()
    {
        // orderList = ownedObjs.Where(x=>x.TryGetComponent(out ITaskable selectable)).Select(x=>x.GetComponent<ITaskable>()).ToList();  
        if(orderList.Count == 0)
        {   
            if(orderButton.image.sprite == nextTurnSprite)
            {
                NextTurn();
                return;
            }   
            orderButton.image.sprite = nextTurnSprite;
            return;
        }
        // orderButton.image.sprite = orderList[orderList.Count-1].OrderSprite;
        orderList[orderList.Count-1].LeftClick();
        UnitManager.Instance.HandleUnitSelected(orderList[orderList.Count-1].Transform);
        Transform targetCameraTransform = orderList[orderList.Count-1].Transform;
        CameraMovement.OnTargetObject?.Invoke(targetCameraTransform);
    }
    public  void GetOrderIcon()
    {
        if(orderList.Count == 0)
        {
            orderButton.image.sprite = nextTurnSprite;
        }
        else
        {
            orderButton.image.sprite = orderList[orderList.Count-1].OrderSprite;
        }
    }
    

    public void ResetOrderIndex()
    {
        orderList = ownedObjs.Where(x=>x.TryGetComponent(out ITaskable selectable)).Select(x=>x.GetComponent<ITaskable>()).ToList();
        foreach (var item in orderList)
        {
            item.TaskReset();
        }
        
    }
}

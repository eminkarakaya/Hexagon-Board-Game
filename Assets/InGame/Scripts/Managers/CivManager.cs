using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Steamworks;
using TMPro;

public abstract class CivManager : NetworkBehaviour
{
    public List<CivManager> savastigiCivler = new List<CivManager>();
    public CivDataUI civDataUI;
    public Side Side;
    [SyncVar] public int team;
    public TMP_Text totalGoldText,goldTextPerRound;
    [SyncVar] public int GoldPerRound ;
    [SyncVar] public int TotalGold ;
    public string nickname;
    protected Button orderButton;
    [SerializeField] protected List<ITaskable> orderList = new List<ITaskable>();
    [SerializeField] protected GameObject buildingPrefab;
    [SyncVar] [SerializeField] public List<GameObject> ownedObjs = new List<GameObject>();
    [SerializeField] private HexGrid hexGrid;
    public CivData civData;
    [SyncVar] public int civType;

    #region  SetGold
    [Command] private void CMDSetGold()
    {
        RPCSetGold();
    }
    [ClientRpc] protected void RPCSetGold()
    {
        TotalGold += GoldPerRound;
        if(isOwned)
            totalGoldText.text = TotalGold.ToString();
    }
    

    public void SetTotalGoldText()
    {
        CMDSetGold();
        
    }
    public void SetGoldPerRoundText()
    {
        if(GoldPerRound > 0)
        {
            goldTextPerRound.text = "+" + GoldPerRound.ToString();
            goldTextPerRound.color = Color.green;
        }
        else if(GoldPerRound< 0)
        {
            goldTextPerRound.text = "-" + GoldPerRound.ToString();
            goldTextPerRound.color = Color.red;
        }
        else
        {
            goldTextPerRound.text = GoldPerRound.ToString();
            goldTextPerRound.color = Color.white;
        }
    }

    #endregion



    [Command]
    public void Capture(NetworkIdentity identity)
    {
        // NetworkServer.ReplacePlayerForConnection(connectionToClient,)
        identity.RemoveClientAuthority();
        identity.AssignClientAuthority(connectionToClient);
    }
    public void DestroyObj(GameObject obj,float dur = 1f)
    {

        CMDRemoveOwnedObject(obj);


        CMDRemoveOrderList(obj,obj);
        
    }
    [Command(requiresAuthority = false)] private void CMDDestroyObj(GameObject obj)
    {

        NetworkServer.Destroy(obj);
    }
    [Command(requiresAuthority = false)] public void CMDSetTeamColor(GameObject obj)
    {
        RPCSetTeamColor(obj);
    }
    [ClientRpc] public void RPCSetTeamColor(GameObject obj)
    {
        TeamColor [] teamColors = obj.GetComponentsInChildren<TeamColor>();
        foreach (var item in teamColors)
        {
            item.SetColor(civData);
        }

    }
    [Command] public void CMDSetTeam(int team)
    {
        this.team = team;
    }









    #region declare war
    [Command(requiresAuthority = false)] public void CMDDeclarePeace(GameObject conn,CivManager civManager)
    {
        NetworkConnectionToClient conn1 = conn.GetComponent<NetworkIdentity>().connectionToClient;
        TargetDeclarePeace(conn1,civManager);
        RPCDeclarePeace(civManager);
    }
    
    [TargetRpc] public void TargetDeclarePeace(NetworkConnectionToClient conn,CivManager civManager)
    {
        civManager.Side = Side.Ally;
        civManager.ownedObjs = civManager.ownedObjs.Where(x=> x!=null).ToList();
        List<ISideable> sideables = civManager.ownedObjs.Where(x=> x.TryGetComponent(out ISideable sideable)).Select(x=>x.GetComponent<ISideable>()).ToList();
        foreach (var item in sideables)
        {
            item.SetSide(Side.None,item.Outline);
        }
        civManager.civDataUI.dealUI.declarePeaceBtn.gameObject.SetActive(false);
        civManager.civDataUI.dealUI.declareWarBtn.gameObject.SetActive(true);
    }
    
    
    [Command(requiresAuthority = false)] public void CMDDeclareWar(GameObject conn,CivManager civManager)
    {
        NetworkConnectionToClient conn1 = conn.GetComponent<NetworkIdentity>().connectionToClient;
        TargetDeclareWar(conn1,civManager);
    }
    [Command] public void CMDDeclareWar2(CivManager civManager)
    {
        RPCDeclareWar(civManager);
        civManager.RPCDeclareWar(this);
    }
    [Command] public void CMDDeclarePeace2(CivManager civManager)
    {
        RPCDeclarePeace(civManager);
        civManager.RPCDeclarePeace(this);
    }
    [ClientRpc] private void RPCDeclarePeace(CivManager civManager)
    {
        if(this.savastigiCivler.Contains(civManager))
            this.savastigiCivler.Remove(civManager);

    }
    [ClientRpc] private void RPCDeclareWar(CivManager civManager)
    {
        if(!this.savastigiCivler.Contains(civManager))
            this.savastigiCivler.Add(civManager);
    }
    
    //1. parametre hangı civmanagerde yapılacagı 2. parametre hangi civ managere savaş acılacagı
    [TargetRpc] public void TargetDeclareWar(NetworkConnectionToClient conn,CivManager civManager)
    {
        civManager.Side = Side.Enemy;
        civManager.ownedObjs = civManager.ownedObjs.Where(x=> x!=null).ToList();
        List<ISideable> sideables = civManager.ownedObjs.Where(x=> x.TryGetComponent(out ISideable sideable)).Select(x=>x.GetComponent<ISideable>()).ToList();
        foreach (var item in sideables)
        {
            item.SetSide(Side.Enemy,item.Outline);
        }
        civManager.civDataUI.dealUI.declareWarBtn.gameObject.SetActive(false);
        civManager.civDataUI.dealUI.declarePeaceBtn.gameObject.SetActive(true);
    }


    #endregion

    #region  ownedObject

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
        
        RemoveOwnedObjTargetRpc(obj);
        
    }
    private void TargetRemoveOrderListAM(GameObject taskable)
    {
        AIManager aIManager = null;
        foreach (var item in FindObjectsOfType<AIManager>())
        {
            if(item.isOwned)
            {
                aIManager = item;
                break;
            }
        }

        taskable.gameObject.SetActive(false);
        CMDDestroyObj(taskable);
        // NetworkServer.Destroy(taskable);
        UnitManager.Instance.ClearOldSelection();
        
    }
    [TargetRpc] private void TargetRemoveOrderListPM(NetworkConnectionToClient conn,GameObject taskable)
    {
        CivManager playerManager = null;
            
            foreach (var item in FindObjectsOfType<PlayerManager>())
            {
                if(item.isOwned)
                {
                    playerManager = item;
                    break;
                }
            }
        Debug.Log(taskable,taskable);
        if(taskable == null) return;
        if(taskable.TryGetComponent(out ITaskable taskable1))
        {
            if(taskable1 == null) return;
            if(playerManager.orderList.Contains(taskable1))
            {
                playerManager.orderList.Remove(taskable1);
            }
            playerManager.GetOrderIcon();
        }
        CMDDestroyObj(taskable);
        UnitManager.Instance.ClearOldSelection();
        // taskable.SetActive(false);
        // Destroy(taskable,1);
    }
    [Command(requiresAuthority = false)] public void CMDRemoveOrderList(GameObject conn, GameObject taskable)
    {
        if(conn.GetComponent<NetworkIdentity>().connectionToClient == null)
        {
            TargetRemoveOrderListAM(taskable);
        }
        else
        {
            CMDHideAllUnits();
            TargetRemoveOrderListPM(conn.GetComponent<NetworkIdentity>().connectionToClient,taskable);
            
            CMDShowAllUnits();
        }

    }
    [TargetRpc] private void RemoveOwnedObjTargetRpc(GameObject obj)
    {
        PlayerManager playerManager = null;
        foreach (var item in FindObjectsOfType<PlayerManager>())
        {
            if(item.isOwned)
            {
                playerManager = item;
            }
        }
        if(playerManager.ownedObjs.Contains(obj))
        {
            playerManager.ownedObjs.Remove(obj);
        }
        // CMDDestroyObj(obj);
    }
    #endregion


    #region  Vision
    // butun playerlerde butun hexlerın visionunu kapatır
    [Command (requiresAuthority = false)]
    public virtual void CMDHideAllUnits()
    {
        RPCHideAllUnits();
    }
    
    // // butun playerlerde butun hexlerın visionunu acar
    [Command(requiresAuthority = false)]
    public virtual void CMDShowAllUnits()
    {
        RPCShowAllUnits();
    }
    [ClientRpc] protected void RPCHideAllUnits()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        // hexGrid.CloseVisible();
        List<Vision> allUnits = FindObjectsOfType<Vision>().ToList();
        foreach (var item in allUnits)
        {
            if(item.TryGetComponent(out IVisionable visionable))
            {
                if(visionable.Hex != null)
                {
                    item.HideVision(visionable.Hex);
                }
            }
        }
    }
    [ClientRpc] protected void RPCShowAllUnits()
    {
        
        hexGrid = FindObjectOfType<HexGrid>();
        hexGrid.CloseVisible();
        List<Vision> allUnits = FindObjectsOfType<Hex>().Select(x=>x.AnyUnit()).Where(x=>x != null ).ToList();
        foreach (var item in allUnits)
        {
            item.ShowVision(item.visionable.Hex);
        }
    }
    #endregion

    public abstract void AddOrderList(ITaskable taskable);
    
    public abstract void RemoveOrderList(ITaskable taskable);

    public abstract void NextRoundBtn();
    public abstract void NextRound();
    
    public abstract void GetOrder();
    
    public abstract void GetOrderIcon();
    
    

    public abstract void ResetOrderIndex();
    
}

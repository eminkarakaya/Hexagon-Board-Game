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
    public CivDataUI civDataUI;
    public Side Side;
    [SyncVar] public int team;
    public TMP_Text totalGoldText,goldTextPerRound;
    [SyncVar] public int GoldPerRound ;
    [SyncVar] public int TotalGold ;
    public string nickname;
    protected Button orderButton;
    [SerializeField] protected Sprite nextRoundSprite,waitingSprite;    
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
    [Command] public void CMDSetTeam(int team)
    {
        this.team = team;
    }
    [Command(requiresAuthority = false)] public void CMDDeclarePeace(GameObject conn,CivManager civManager)
    {
        NetworkConnectionToClient conn1 = conn.GetComponent<NetworkIdentity>().connectionToClient;
        TargetDeclarePeace(conn1,civManager);
        RPCDeclarePeace(civManager);
    }
    [ClientRpc] public void RPCDeclarePeace(CivManager civManager)
    {
        // buton degısıklıgı
        civManager.civDataUI.dealUI.declarePeaceBtn.onClick.AddListener(()=>civDataUI.dealUI.declarePeaceBtn.gameObject.SetActive(false));
        civManager.civDataUI.dealUI.declarePeaceBtn.onClick.AddListener(()=>civDataUI.dealUI.declareWarBtn.gameObject.SetActive(true));

    }
    [TargetRpc] public void TargetDeclarePeace(NetworkConnectionToClient conn,CivManager civManager)
    {
        List<ISideable> sideables = civManager.ownedObjs.Where(x=> x.TryGetComponent(out ISideable sideable)).Select(x=>x.GetComponent<ISideable>()).ToList();
        // List<ISideable> sideables = ownedObjs.Where(x=> x.TryGetComponent(out ISideable sideable)).Select(x =>x.GetComponent<ISideable>()).ToList();
        foreach (var item in sideables)
        {
            item.SetSide(Side.None,item.Outline);
        }
    }
    
    [Command(requiresAuthority = false)] public void CMDDeclareWar(GameObject conn,CivManager civManager)
    {
        NetworkConnectionToClient conn1 = conn.GetComponent<NetworkIdentity>().connectionToClient;
        TargetDeclareWar(conn1,civManager);
    }
    [ClientRpc] public void RPCDeclareWar(CivManager civManager)
    {
        civManager.civDataUI.dealUI.declareWarBtn.onClick.AddListener(()=>civDataUI.dealUI.declareWarBtn.gameObject.SetActive(false));
        civManager.civDataUI.dealUI.declareWarBtn.onClick.AddListener(()=>civDataUI.dealUI.declarePeaceBtn.gameObject.SetActive(true));
    }
    //1. parametre hangı civmanagerde yapılacagı 2. parametre hangi civ managere savaş acılacagı
    [TargetRpc] public void TargetDeclareWar(NetworkConnectionToClient conn,CivManager civManager)
    {
        
        List<ISideable> sideables = civManager.ownedObjs.Where(x=> x.TryGetComponent(out ISideable sideable)).Select(x=>x.GetComponent<ISideable>()).ToList();
        foreach (var item in sideables)
        {
            item.SetSide(Side.Enemy,item.Outline);
        }
    }

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
        RPCRemoveOwnedObj(obj);
    }
    [ClientRpc] private void RPCRemoveOwnedObj(GameObject obj)
    {

        if(ownedObjs.Contains(obj))
        {
            ownedObjs.Remove(obj);
        }
    }
    #endregion


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
        List<Vision> allUnits = FindObjectsOfType<Hex>().Select(x=>x.AnyUnit()).Where(x=>x != null ).ToList();
        foreach (var item in allUnits)
        {
            // Debug.Log(item + " a " + item.visionable + " a " + item.visionable.Hex , item);
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

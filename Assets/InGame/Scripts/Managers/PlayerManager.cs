using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;
using TMPro;
using Steamworks;
using UnityEngine.SceneManagement;
using System;
public class PlayerManager : CivManager
{

    #region  properties
    [SyncVar] public bool isStart;
    public Action NextRoundEvent;


    public TooltipTrigger tooltipTrigger;
    public TMP_Text tipText;
    public const string NETX_ROUND_STRING = "Next Round",UNIT_NEEDS_ORDER = "Unit Needs Orders", WAITING_OTHER_PLAYERS = "Waiting Other Players";
    public List<PlayerManager> waitedPlayers = new List<PlayerManager>();
    public GameManager gameManager;
    private NetworkManagerGdd room;
    private NetworkManagerGdd Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerGdd;
        }
    }
    

    // public List<ITaskable> liveUnits = new List<ITaskable>();
    [SerializeField] private GameObject civUIPrefab;
    /* PlayerInfoDisplay */
    
    
  
    #endregion


    #region  unityMirrorCallbacks
    // private void Awake() {
    //     NetworkIdentity [] identities = FindObjectsOfType<NetworkIdentity>();
    //     foreach (var item in identities)
    //     {
    //         item.gameObject.SetActive(true);
    //     }
    // }
    private void Start() {
        
    }

    [Command] private void SetIsStart()
    {
        isStart = true;
    }
    public IEnumerator StartGame()
    {
        if(isOwned)
        {
            SetIsStart();
            while(gameManager == null)
            {
                Debug.Log("GAMEMANAGER");
                gameManager = GameManager.instance;
                yield return null;
            }
            while(gameManager.playerCount != FindObjectsOfType<PlayerManager>().Where(x=>x.isStart == true).Count())
            {
                Debug.Log("ISSTART");
                yield return null;
            }
            // if(isOwned)
            //     CMDSetCivData();
            while(civData == null)
            {
                Debug.Log("CIVDATA");
                yield return null;
            }
            
            waitedPlayers = FindObjectsOfType<PlayerManager>().ToList();
            orderButton = gameManager.OrderButton;
            orderButton.onClick.AddListener(GetOrder);
            orderButton.image.sprite = GameSettingsScriptable.Instance.nextRoundSprite;
            tipText = gameManager.nextRoundTipText;
            tooltipTrigger = gameManager.tooltipTrigger;
            totalGoldText = gameManager.goldText;
            goldTextPerRound = gameManager.goldPerRoundText;
            gameManager.ownedPlayerManager = this;
            CMDCreateCivUI(gameManager);
            CMDCreateBuilding(gameManager);
            GetOrderIcon();
            StartCoroutine(wait1());
        }

    }
    IEnumerator wait1()
    {
        yield return new WaitForSeconds(2);
        foreach (var item in FindObjectsOfType<CivDataUI>())
        {
            item.SetDeclareWarButton(this);
        }
    }
    public override void OnStartClient()
    {
        // DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);

        if(isOwned)
        {
           
            FindObjectOfType<SelectCiv>().button.onClick.AddListener(()=> StartCoroutine(StartGame()));
            
        }
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }


    [Command] public void CMDSetCivData(int civDataIndex,GameManager gameManager)
    {
        RPCSetCivData(civDataIndex,gameManager);
    }
    [ClientRpc] private void RPCSetCivData(int civDataIndex, GameManager gameManager)
    {
        // gameManager = FindObjectOfType<GameManager>();
        this.civData = gameManager.GetCivData(civDataIndex);
        // this.civType = index;
    }

    #endregion

    #region lobby team falan


    


    #endregion



    [Command] public void CMDSetName(string str)
    {
        RPCSetName(str);
    }
    [ClientRpc] private void RPCSetName(string str)
    {
        nickname = str;
    }
    [Command] private void CMDCreateCivUI(GameManager gameManager)
    {
        CivDataUI civDataUI = Instantiate(civUIPrefab,gameManager.civUIParent).GetComponent<CivDataUI>();
        NetworkServer.Spawn(civDataUI.gameObject,connectionToClient);
        RPGCreateCivUI(civDataUI);
    }
    [ClientRpc] private void RPGCreateCivUI(CivDataUI civDataUI)
    {
        civDataUI.civManager = this;
        civDataUI.civData = civData;
        civDataUI.civImage.sprite = civData.civImage;
        civDataUI.SetNicknameText();

        gameManager = FindObjectOfType<GameManager>();
        foreach (var item in FindObjectsOfType<CivDataUI>().ToList())
        {
            item.transform.SetParent(gameManager.civUIParent);
        }
    }

    #region  createBuilding
    [Command] // client -> server
    private void CMDCreateBuilding(GameManager gameManager)
    {
        Building building = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(building.gameObject,connectionToClient);

        RPCCreateBuilding(building,gameManager.hexIndex);
        gameManager.hexIndex ++;
        FindPlayerManager(building);
    }

    [ClientRpc] // server -> client
    private void RPCCreateBuilding(Building building,int i)
    {

        if( gameManager.playersHexes[i].Building != null)
        {
            i++;    
        }
        gameManager.buildings = FindObjectsOfType<Building>().ToList();
        building.transform.position = new Vector3 (gameManager.playersHexes[i]. transform.position.x , 1 , gameManager.playersHexes[i]. transform.position.z );
        building.transform.rotation = Quaternion.Euler(-90,0,0);

        building.Hex = gameManager.playersHexes[i];
        building.Hex.Building = building;
        ownedObjs.Add(building.gameObject);
        foreach (var item in gameManager.buildings)
        {
            if(item == null) continue;
            if(item.isOwned)
            {
                item.SetSide(Side.Me,item.GetComponent<OutlineObj>());
            }
            else
                item.SetSide(Side.None,item.GetComponent<OutlineObj>());
        }
        
        var managers = FindObjectsOfType<PlayerManager>();

        CMDSetTeamColor(building.gameObject);
    }
    [Command] private void qwe()
    {
        
    }
    #endregion

    #region  findPlayerMnaager
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
        building.CivManager.CMDSetTeamColor(this.gameObject);

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
    #endregion

    #region  hideshow
    [Command(requiresAuthority = false)]
    public override void CMDHideAllUnits()
    {
        RPCHideAllUnits();
    }

    [Command(requiresAuthority = false)]
    public override void CMDShowAllUnits()
    {
        RPCShowAllUnits();
    }
    #endregion

    #region  steam falan
   
    
    
    

    // IEnumerator wait()
    // {
    //     while(!SceneManager.GetSceneByBuildIndex(1).isLoaded)
    //     {
    //         yield return null;
    //     }
    //     SceneManager.UnloadSceneAsync(0);

    // }
    #endregion
    /* playerınfodisplay */




    #region  round order
    public override void GetOrderIcon()
    {
        if(orderList.Count == 0)
        {
            orderButton.image.sprite = GameSettingsScriptable.Instance.nextRoundSprite;
            tipText.text = NETX_ROUND_STRING;
        }
        else
        {
            orderButton.image.sprite = orderList[orderList.Count-1].OrderSprite;
            tipText.text = UNIT_NEEDS_ORDER;
        }
    }

    public override void ResetOrderIndex()
    {
        ownedObjs = ownedObjs.Where(x=> x!=null).ToList();
        orderList = ownedObjs.Where(x=> x.TryGetComponent(out ITaskable selectable) ).Select(x=>x.GetComponent<ITaskable>()).ToList();
        foreach (var item in orderList)
        {
            item.TaskReset();
        }
    }
    public override void GetOrder()
    {
        if(orderList.Count == 0)
        {
            // orderlist bıttıyse

            // next round butonuna basıldı mı basılmadı mı
            if(orderButton.image.sprite == GameSettingsScriptable.Instance.nextRoundSprite)
            {
                NextRoundBtn();
                UnitManager.Instance.ClearOldSelection();
                
                return;
            }
            if(orderButton.image.sprite == GameSettingsScriptable.Instance.waitingSprite)
            {
                CMDAddWaitingList();
                
                orderButton.image.sprite = GameSettingsScriptable.Instance.nextRoundSprite;
                return;
            }
            orderButton.image.sprite = GameSettingsScriptable.Instance.nextRoundSprite;
            return;
        }
        // sıradakı objeyı seciyo
        while( orderList[orderList.Count-1] == null)
        {
            orderList.RemoveAt(orderList.Count-1);
            GetOrder();
        }
        orderList[orderList.Count-1].LeftClick();
        UnitManager.Instance.HandleUnitSelected(orderList[orderList.Count-1].Transform);
        Transform targetCameraTransform = orderList[orderList.Count-1].Transform;
        CameraMovement.OnTargetObject?.Invoke(targetCameraTransform);
    }
    [ClientRpc] private void RPCNextTourBTN()
    {
        RemoveWaitingList();
        if(isOwned)
        {
            orderButton.image.sprite = GameSettingsScriptable.Instance.waitingSprite;
            tipText.text = WAITING_OTHER_PLAYERS;
        }
            
        foreach (var item in FindObjectsOfType<PlayerManager>())
        {   
            if(item.waitedPlayers.Count == 0)
            {
                // SetWaitedListTip();
                if(item.isOwned)
                {
                    item.NextRound();
                }
            }
        }

    }
    private void CloseHoverTip()
    {
        tooltipTrigger.content = string.Empty;
    }

    public void SetWaitedListTip()
    {
        
        foreach (var item in FindObjectsOfType<PlayerManager>())
        {
            if(item.isOwned)
            {
                string str = string.Empty;
                foreach (var item1 in item.waitedPlayers)
                {
                    str += item1.nickname + ", ";
                }
                if(str == string.Empty) return;
                if(str.Length < 3) return;
                str.Remove(str.Count()-3,3);
                item.tooltipTrigger.content = string.Empty;
                item.tooltipTrigger.content += str;
            }
        }
    }
    [Command] public void CMDNextRoundBTN()
    {
        RPCNextTourBTN();
    }

    public void RemoveWaitingList()
    {
        foreach (var item in FindObjectsOfType<PlayerManager>())
        {
            if(item.isOwned)
            {
                if(item.waitedPlayers.Contains(this))
                {
                    item.waitedPlayers.Remove(this);
                }
            }
        }
        SetWaitedListTip();
    }
    [Command]
    public void CMDAddWaitingList()
    {
        RPCAddWaitingList();
    }
    [ClientRpc]
    public void RPCAddWaitingList()
    {
        foreach (var item in FindObjectsOfType<PlayerManager>())
        {
            if(item.isOwned)
            {
                if(!item.waitedPlayers.Contains(this))
                {
                    item.waitedPlayers.Add(this);
                }
            }
        }
        SetWaitedListTip();
        if(isOwned)
            CloseHoverTip();
        // if(!waitedPlayers.Contains(this))
        // {
        //     waitedPlayers.Add(this);
        // }
    }
    // TUR BITIMI
    public override void NextRound()
    {
        NextRoundEvent?.Invoke();
        waitedPlayers = FindObjectsOfType<PlayerManager>().ToList();
        ResetOrderIndex();
        GetOrderIcon();
        // TotalGold += GoldPerTurn;
        SetTotalGoldText();
        // SetWaitedListTip();
    }



    public override void AddOrderList(ITaskable taskable)
    {
        if(!orderList.Contains(taskable))
        {
            orderList.Add(taskable);
        }
    }

    public override void RemoveOrderList(ITaskable taskable)
    {
        if(orderList.Contains(taskable))
        {
            orderList.Remove(taskable);
        }
        

        GetOrderIcon();
    }

    public override void NextRoundBtn()
    {
        CMDNextRoundBTN();
        

    }
    #endregion

    
}

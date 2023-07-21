using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;
using TMPro;
using Steamworks;
using UnityEngine.SceneManagement;
public class PlayerManager : CivManager
{
    #region  properties
    public List<PlayerManager> waitedPlayers = new List<PlayerManager>();
    public GameManager gameManager;
    [SerializeField] PlayerInfoDisplay lobbyPrefab;
    public PlayerInfoDisplay lobby;
    [SyncVar] public int team;

    public Side Side;
    // public List<ITaskable> liveUnits = new List<ITaskable>();
    [SerializeField] private GameObject civUIPrefab;
    /* PlayerInfoDisplay */ 
    public LobbyItem lobbyItemPrefab;

    LobbyItem lobbyItem;
    [SyncVar(hook =nameof(HandleSteamIDUpdated))] private ulong steamID;
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;
    #endregion

    
   

    #region  unityMirrorCallbacks
    private void Awake() {
        team = Random.Range(0,2);

        // DontDestroyOnLoad(this.gameObject);
    }
   
    public IEnumerator StartGame()
    {

        while(gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            yield return null;
        }
        while(gameManager.playerCount != FindObjectsOfType<PlayerManager>().Length)
        {
            Debug.Log("waiting other players..");
            yield return null;
        }
        if(isOwned)
        {
            waitedPlayers = FindObjectsOfType<PlayerManager>().ToList();
            orderButton = gameManager.OrderButton;
            orderButton.onClick.AddListener(GetOrder);
            orderButton.image.sprite = nextTurnSprite;
            CMDCreateCivUI();
            CMDCreateBuilding();
            // StartCoroutine(wait());
            StartCoroutine(Wait());
            
        }    
    }

    #endregion

    #region lobby team falan

    [ClientRpc] private void RPCSetParentLobby()
    {
        lobby.gameObject.SetActive(true);
        lobby.transform.SetParent(SteamNetworkManager.instance.playerPrefabParent);
    }
    [Command] public void CreateLobbyItem()
    {

        GameObject _lobby = Instantiate(lobbyPrefab).gameObject;
        NetworkServer.Spawn(_lobby,connectionToClient);
        lobby = _lobby.GetComponent<PlayerInfoDisplay>();
        RPCSetParentLobby();
    }
    
    private IEnumerator Wait()
    {
        while(team == -1)
        {
            yield return null;
        }
        CMDSetSideAll();
    }
    [Command] public void CMDSetSideAll()
    {
        RPC();
    }
    [ClientRpc]
    private void RPC()
    {
        SetSideAllManagers();
    }


    private void SetSideAllManagers()
    {
        var managers = FindObjectsOfType<PlayerManager>();
        foreach (var item1 in managers)
        {
            foreach (var item in managers)
            {
                if(item.isOwned)
                {
                    item.Side = Side.Me;
                    continue;
                }    
                if(item.team == item1.team)
                {
                    item.Side = Side.Ally;
                }
                else
                    item.Side = Side.Enemy;
            }
        }
    }
    #endregion
   
   
    

    [Command] private void CMDCreateCivUI()
    {
        CivDataUI civDataUI = Instantiate(civUIPrefab,gameManager.civUIParent).GetComponent<CivDataUI>();
        NetworkServer.Spawn(civDataUI.gameObject,connectionToClient);
        RPGCreateCivUI(civDataUI);
        
    }
    [ClientRpc] private void RPGCreateCivUI(CivDataUI civDataUI)
    {
        civDataUI.civManager = this;
        civDataUI.civData = civData;
        foreach (var item in FindObjectsOfType<CivDataUI>().ToList())
        {
            item.transform.SetParent(gameManager.civUIParent);
        }
    }
    
    #region  createBuilding
    [Command] // client -> server
    private void CMDCreateBuilding()
    {
        Building building = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(building.gameObject,connectionToClient);
        
       
        RPCCreateBuilding(building,gameManager.playersHexes.Count-1);
        
        FindPlayerManager(building);
    }
    
    [ClientRpc] // server -> client
    private void RPCCreateBuilding(Building building,int i)
    {
        
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
                item.SetSide(Side.Me,item.GetComponent<Outline>());
            }
            else
                item.SetSide(Side.Enemy,item.GetComponent<Outline>());  
        }
        var managers = FindObjectsOfType<PlayerManager>();
       
        gameManager.playersHexes.RemoveAt(gameManager.playersHexes.Count-1);
        SetTeamColor(building.gameObject);
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
        building.CivManager.SetTeamColor(this.gameObject);

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
    #endregion
   
    


    /* playerınfodisplay */
    public override void OnStartClient()
    {  
        StartCoroutine (StartGame());

        // if(lobbyItem == null)
        //     lobbyItem = Instantiate(lobbyItemPrefab,SteamNetworkManager.instance.playerPrefabParent); 
        // avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }
    #region  steam falan
    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID != steamID) return;
        lobbyItem.profileImage.texture = GetSteamImageAsTexture(callback.m_iImage);
    }

    private void HandleSteamIDUpdated(ulong oldId,ulong newId)
    {
        if(lobbyItem == null)
        {
            lobbyItem = Instantiate(lobbyItemPrefab,SteamNetworkManager.instance.playerPrefabParent); 

        }
        var cSteamID = new CSteamID(newId);
        lobbyItem.displayNameText.text = SteamFriends.GetFriendPersonaName(cSteamID);
        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamID);
        if(imageId == -1) return;
        lobbyItem.profileImage.texture =  GetSteamImageAsTexture(imageId);
    }
    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage,out uint width,out uint height);
        if(isValid)
        {
            byte[] image = new byte[width*height*4];
            isValid = SteamUtils.GetImageRGBA(iImage,image,(int)(width*height*4));
            if(isValid)
            {
                texture = new Texture2D((int)width,(int)height,TextureFormat.RGBA32,false,true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        return texture;
    }
    public void SetSteamId(ulong steamID)
    {
        this.steamID = steamID;
    }
    [Command]
    public void CMDBeginGame()
    {
        TargetBeginGame();
    }
    [TargetRpc]
    public void TargetBeginGame () {
        //Additively load game scene
        // SceneManager.LoadScene (1,LoadSceneMode.Additive);

        // lobby.gameObject.SetActive(false);
        // StartCoroutine (StartGame());
        // StartCoroutine(wait());
    }

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




    #region 
    public override void GetOrderIcon()
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

    public override void ResetOrderIndex()
    {
        orderList = ownedObjs.Where(x=>x.TryGetComponent(out ITaskable selectable)).Select(x=>x.GetComponent<ITaskable>()).ToList();
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

            // next turn butonuna basıldı mı basılmadı mı 
            if(orderButton.image.sprite == nextTurnSprite)
            {
                CMDNextTurnBTN();
                return;
            }   
            orderButton.image.sprite = nextTurnSprite;
            return;
        }

        // sıradakı objeyı seciyo
        orderList[orderList.Count-1].LeftClick();
        UnitManager.Instance.HandleUnitSelected(orderList[orderList.Count-1].Transform);
        Transform targetCameraTransform = orderList[orderList.Count-1].Transform;
        CameraMovement.OnTargetObject?.Invoke(targetCameraTransform);
    }
    [ClientRpc] private void RPCNextTurnBTN()
    {
        RPCRemoveWaitingList();
        if(isOwned)
            orderButton.image.sprite = waitingSprite;
        foreach (var item in FindObjectsOfType<PlayerManager>())
        {
            if(item.waitedPlayers.Count == 0 && item.isOwned)
            {
                item.NextTurn();
            }
        }
    }
    [Command] public void CMDNextTurnBTN()
    {
        RPCNextTurnBTN();
    }
   
    public override void NextTurnBtn()
    {
        
    }

    public void RPCRemoveWaitingList()
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
    }
    [Command]
    public void CMDAddWaitingList()
    {
        RPCAddWaitingList();
    }
    [ClientRpc]
    public void RPCAddWaitingList()
    {
        if(!waitedPlayers.Contains(this))
        {
            waitedPlayers.Add(this);
        }
    }
    public override void NextTurn()
    {
            Debug.Log("NEXT TURN");
        
        waitedPlayers = FindObjectsOfType<PlayerManager>().ToList();
        ResetOrderIndex();
        GetOrderIcon();
        
        // 
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
    #endregion
    #region  turn order


    #endregion
}

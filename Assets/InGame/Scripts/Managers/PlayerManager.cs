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
    public GameManager gameManager;
    [SerializeField] PlayerInfoDisplay lobbyPrefab;
    public PlayerInfoDisplay lobby;
    [SyncVar] public int team;

    public Side Side;
    public List<IMovable> liveUnits;
    [SerializeField] private GameObject civUIPrefab;
    /* PlayerInfoDisplay */ 
    public LobbyItem lobbyItemPrefab;

    LobbyItem lobbyItem;
    [SyncVar(hook =nameof(HandleSteamIDUpdated))] private ulong steamID;
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;
    /* PlayerInfoDisplay */ 

    private void Awake() {
        team = Random.Range(0,2);
        // DontDestroyOnLoad(this.gameObject);
    }
    public IEnumerator StartGame()
    {
        if(isOwned)
        {
            while(gameManager == null)
            {
                Debug.Log("waiting gm");
                gameManager = gameManager = FindObjectOfType<GameManager>();
                yield return null;
            }
            waitGameManager();
            CMDCreateBuilding();
            // StartCoroutine(wait());
            StartCoroutine(Wait());
            
        }    
    }
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
    
    private void Start() {
        StartCoroutine (StartGame());
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
    public void SetSide(Side side, Outline outline)
    {
        this.Side = side;
        if(outline == null) return;
        if(side == Side.Me)
        {
            outline.OutlineColor = Color.white;
        }
        else if(side == Side.Enemy)
        {
            outline.OutlineColor = Color.red;
        }
        else
            outline.OutlineColor = Color.blue;
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
    IEnumerator waitGameManager()
    {
        while(gameManager == null)
        {
            Debug.Log("waiting gm");
            gameManager = gameManager = FindObjectOfType<GameManager>();
            yield return null;
        }
        CMDCreateCivUI();
    }
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
    
    [Command] // client -> server
    private void CMDCreateBuilding()
    {
        Building building = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(building.gameObject,connectionToClient);
        
        ownedObjs.Add(building.gameObject);
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
    
    /* playerınfodisplay */
    public override void OnStartClient()
    {  
        

        // if(lobbyItem == null)
        //     lobbyItem = Instantiate(lobbyItemPrefab,SteamNetworkManager.instance.playerPrefabParent); 
        // avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

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
        SceneManager.LoadScene (1,LoadSceneMode.Additive);

        lobby.gameObject.SetActive(false);
        StartCoroutine (StartGame());
        StartCoroutine(wait());
    } 
    IEnumerator wait()
    {
        while(!SceneManager.GetSceneByBuildIndex(1).isLoaded)
        {
            Debug.Log("waitin Scene load");
            yield return null;
        }
        SceneManager.UnloadSceneAsync(0);

    }
        /* playerınfodisplay */
}

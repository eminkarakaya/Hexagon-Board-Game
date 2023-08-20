using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class CreateLobbySteam : NetworkBehaviour
{
    public static CreateLobbySteam instance;
    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
   [SerializeField]private string HostAddressKey = "HostAddress";
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<LobbyEnter_t> lobbyEnter;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyMatchList_t> lobbyMatchList;
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdated;
    public List<CSteamID> lobbyIDs = new List<CSteamID>();
    [SerializeField] private GameObject buttons = null;
    [SerializeField] private SteamNetworkManager networkManager;
    public static CSteamID LobbyId {get; private set;}
    private void Start() {
        if(!SteamManager.Initialized) return;
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        lobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }
    public void GetLobbiesList()
    {
        if(lobbyIDs.Count > 0) lobbyIDs.Clear();
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(500);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnGetLobbyData(LobbyDataUpdate_t callback)
    {
        LobbiesListManager.instance.DisplayLobbies(lobbyIDs,callback);
    }

    private void OnGetLobbyList(LobbyMatchList_t callback)
    {
        if(LobbiesListManager.instance.listOfLobbies.Count > 0) LobbiesListManager.instance.DestroyLobbies();

        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyId);
            SteamMatchmaking.RequestLobbyData(lobbyId);
        }
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if(NetworkServer.active) return;
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID (callback.m_ulSteamIDLobby),HostAddressKey);
        if(string.IsNullOrEmpty(hostAddress))
        {
            Debug.Log("null");
        }        
        networkManager.networkAddress = hostAddress;
        StartCoroutine(Wait(hostAddress));
    }
    private IEnumerator Wait(string hostAddress)
    {
        while(networkManager.networkAddress != hostAddress)
        {
            yield return null;
        }
        networkManager.StartClient();
        buttons.SetActive(false);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    public void HostLobby()
    {
        buttons.SetActive(false);
        if(networkManager == null)
            networkManager = FindObjectOfType<SteamNetworkManager>();
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic,networkManager.maxConnections);
        
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK) 
        {
            buttons.SetActive(true);
            return;
        }
        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),HostAddressKey,SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),"name",SteamFriends.GetPersonaName().ToString() + "' S LOBBY");
        
    }
    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }
    
    public void BeginGame()
    {
        foreach (var item in SteamNetworkManager.LocalPlayers.Values)
        {
            item.CMDBeginGame();
        }
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Linq;
using System.Collections;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public class NetworkManagerGdd : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab;
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab;
    [SerializeField] private GameObject spawnSystem;

    public static new NetworkManagerGdd singleton { get; private set; }
    
    [SerializeField] private string menuScene = "MenuScene";

    public static event Action OnClientConnected,OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnSceneChangedEvent;

    public List<NetworkRoomPlayerLobby> RoomPlayers {get;} = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers {get;} = new List<NetworkGamePlayerLobby>();
    public void NotifyPlayersOnReadyState()
    {
        foreach (var item in RoomPlayers)
        {
            item.HandleReadyToStart(IsReadyToStart());
        }
    }
    private bool IsReadyToStart()
    {
        if(numPlayers < minPlayers) return false;
        
        foreach (var item in RoomPlayers)
        {
            if(!item.IsReady) return false;
        }
        return true;
    }
    public void StartGame()
    {
        if(SceneManager.GetActiveScene().name == menuScene)
        {
            if(!IsReadyToStart()) return;
            ServerChangeScene("Scene1");
        }
    }
    [ContextMenu("ASSIGN NETWORK OBJECTS")]
    public void AssignNetworkObjects()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("").Where(x=>x.TryGetComponent(out NetworkIdentity networkIdentity) && !x.TryGetComponent(out Player playerManager)).ToList();
    }
    #region Unity Callbacks
    
    public override void Awake()
    {
        base.Awake();
        singleton = this;
    }
    public override void OnValidate()
    {
        base.OnValidate();
        #if UNITY_EDITOR
        
        #endif
    }
    public override void Start()
    {
        base.Start();
        
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region Start & Stop

    /// <summary>
    /// Set the frame rate for a headless server.
    /// <para>Override if you wish to disable the behavior or set your own tick rate.</para>
    /// </summary>
    public override void ConfigureHeadlessFrameRate()
    {
        base.ConfigureHeadlessFrameRate();
    }

    /// <summary>
    /// called when quitting the application by closing the window / pressing stop in the editor
    /// </summary>
    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// This causes the server to switch scenes and sets the networkSceneName.
    /// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready. The clients must call NetworkClient.Ready() again to participate in the new scene.</para>
    /// </summary>
    /// <param name="newSceneName"></param>
    public override void ServerChangeScene(string newSceneName)
    {
        if(SceneManager.GetActiveScene().name == menuScene)
        {
            for (int i = RoomPlayers.Count - 1; i>=0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn,gameplayerInstance.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    /// <summary>
    /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows server to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    public override void OnServerChangeScene(string newSceneName) { }

    /// <summary>
    /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName">The name of the new scene.</param>
    public override void OnServerSceneChanged(string sceneName) { 

        if(sceneName.StartsWith("Scene1"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(spawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
        // base.OnServerSceneChanged(sceneName);
        // if(sceneName == scene)
        // {
        //     StartCoroutine(ServerLoadSubScene());
        // }
    }

    /// <summary>
    /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    /// <param name="sceneOperation">Scene operation that's about to happen</param>
    /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { 
        
    }

    /// <summary>
    /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
    /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
    /// </summary>
    public override void OnClientSceneChanged()
    {
        if(SceneManager.GetActiveScene().name == "Scene1")
        {
            OnSceneChangedEvent?.Invoke();
            
        }
        base.OnClientSceneChanged();
        


    }

    #endregion

    #region Server System Callbacks

    /// <summary>
    /// Called on the server when a new client connects.
    /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerConnect(NetworkConnectionToClient conn) { 
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        // if(SceneManager.GetActiveScene().name != menuScene)
        // {
        //     conn.Disconnect();
        //     return;
        // }
        // base.OnServerConnect(conn);
    }

    /// <summary>
    /// Called on the server when a client is ready.
    /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn);
    }

    /// <summary>
    /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

        NetworkServer.AddPlayerForConnection(conn, player);
        // LocalPlayers.Add(conn,player.GetComponent<Player>());
        // base.OnServerAddPlayer(conn);
        if(SceneManager.GetActiveScene().name == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;
            NetworkRoomPlayerLobby roomPlayerLobby = Instantiate(roomPlayerPrefab);
            NetworkServer.Spawn(roomPlayerLobby.gameObject,conn);
            roomPlayerLobby.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn,roomPlayerLobby.gameObject);
        }
    }


    /// <summary>
    /// Called on the server when a client disconnects.
    /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            RoomPlayers.Remove(player);
            NotifyPlayersOnReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// Called on server when transport raises an exception.
    /// <para>NetworkConnection may be null.</para>
    /// </summary>
    /// <param name="conn">Connection of the client...may be null</param>
    /// <param name="exception">Exception thrown from the Transport.</param>
    public override void OnServerError(NetworkConnectionToClient conn, TransportError transportError, string message) { }

    #endregion

    #region Client System Callbacks

    /// <summary>
    /// Called on the client when connected to a server.
    /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    /// </summary>
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    /// <summary>
    /// Called on clients when disconnected from a server.
    /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
    /// </summary>
    public override void OnClientDisconnect() { 
        OnClientDisconnected?.Invoke();

    }

    /// <summary>
    /// Called on clients when a servers tells the client it is no longer ready.
    /// <para>This is commonly used when switching scenes.</para>
    /// </summary>
    public override void OnClientNotReady() { }

    /// <summary>
    /// Called on client when transport raises an exception.</summary>
    /// </summary>
    /// <param name="exception">Exception thrown from the Transport.</param>
    public override void OnClientError(TransportError transportError, string message) { }

    #endregion

    #region Start & Stop Callbacks

    // Since there are multiple versions of StartServer, StartClient and StartHost, to reliably customize
    // their functionality, users would need override all the versions. Instead these callbacks are invoked
    // from all versions, so users only need to implement this one case.

    /// <summary>
    /// This is invoked when a host is started.
    /// <para>StartHost has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartHost() { }

    /// <summary>
    /// This is invoked when a server is started - including when a host is started.
    /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartServer() { 
    }

    /// <summary>
    /// This is invoked when the client is started.
    /// </summary>
    public override void OnStartClient() { 

        AssignNetworkObjects();
        foreach (var item in spawnPrefabs)
        {
            NetworkClient.RegisterPrefab(item);
        }
    }

    /// <summary>
    /// This is called when a host is stopped.
    /// </summary>
    public override void OnStopHost() { }

    /// <summary>
    /// This is called when a server is stopped - including when a host is stopped.
    /// </summary>
    public override void OnStopServer() { 
        RoomPlayers.Clear();
    }

    /// <summary>
    /// This is called when a client is stopped.
    /// </summary>
    public override void OnStopClient() { }
    
    #endregion
}

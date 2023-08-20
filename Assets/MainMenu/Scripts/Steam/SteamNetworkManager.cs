using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
public class SteamNetworkManager : NetworkManager
{
    public static SteamNetworkManager instance;
    public static Dictionary<NetworkConnection,PlayerManagerLobby> LocalPlayers = new Dictionary<NetworkConnection, PlayerManagerLobby>();
    public Transform playerPrefabParent;
    public override void Awake()
    {
        base.Awake();
        instance = this;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // base.OnServerAddPlayer(conn);
        Transform startPos = GetStartPosition();
            GameObject player = startPos != null ? Instantiate(playerPrefab,startPos.position,startPos.rotation):Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn,player);
        CSteamID steamID = SteamMatchmaking.GetLobbyMemberByIndex(CreateLobbySteam.LobbyId,numPlayers-1);
        var playerInfoDisplay = conn.identity.GetComponent<PlayerManagerLobby>().lobby;
        conn.identity.GetComponent<PlayerManagerLobby>().CreateLobbyItem();
        StartCoroutine (wait(playerInfoDisplay,conn,player,steamID));
    }
    private IEnumerator wait(PlayerInfoDisplay playerInfoDisplay,NetworkConnectionToClient conn,GameObject player,CSteamID steamID)
    {
        while(playerInfoDisplay == null)
        {
            playerInfoDisplay = conn.identity.GetComponent<PlayerManagerLobby>().lobby;
            yield return null;
        }
        playerInfoDisplay.SetSteamId(steamID.m_SteamID); 
        LocalPlayers[conn] = player.GetComponent<PlayerManagerLobby>();

    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        LocalPlayers.Remove(conn);
    }
   

}

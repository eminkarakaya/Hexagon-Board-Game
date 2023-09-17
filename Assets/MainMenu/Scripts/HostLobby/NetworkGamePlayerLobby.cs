using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar] private string displayName = "Loading...";
    public bool IsReady = false;
     private NetworkManagerGdd room;
    public NetworkManagerGdd Room
    {
        get
        {
            if(room != null) return room;
            return room = NetworkManagerGdd.singleton as NetworkManagerGdd;
        }
        
    }
    public override void OnStartClient()
    {
        Room.GamePlayers.Add(this);
    }
    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }
    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}

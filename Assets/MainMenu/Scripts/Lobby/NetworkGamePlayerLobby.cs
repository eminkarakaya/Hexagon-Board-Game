using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "Loading...";

    

    
    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}

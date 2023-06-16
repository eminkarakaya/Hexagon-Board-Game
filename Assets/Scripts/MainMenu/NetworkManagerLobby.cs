using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using System;
using UnityEngine.SceneManagement;


public class NetworkManagerLobby : NetworkManager
{
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        StartCoroutine(WaitInstance());
    }
    IEnumerator WaitInstance()
    {
        while(RoomSettings.Instance == null)
        {
            yield return null;
        }
        // RoomSettings.Instance.allClients.Add(playerPrefab.GetComponent<PlayerMainMenu>());
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerMainMenu : NetworkBehaviour
{
    [SyncVar] public string DisplayName;
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab;
    [SerializeField] private RoomSettings roomSettingsPrefab,roomSettings1;
    private void Start() {
        
    }
    
    public override void OnStartClient()
    {
        if(isOwned)
            CreatePrefab();
        
        if(isServer && isOwned)
        {
            roomSettings1= Instantiate(roomSettingsPrefab);
            NetworkServer.Spawn(roomSettings1.gameObject,connectionToClient);
        }
    }

    [Command]
    private void CreatePrefab()
    {
        NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
        NetworkServer.Spawn(roomPlayerInstance.gameObject,connectionToClient);
        qe(roomPlayerInstance);
    }
    [ClientRpc]
    private void qe(NetworkRoomPlayerLobby roomPlayerLobby)
    {
        // roomSettings1.allClients.Add(this);
            DisplayName = PlayerNameInput.Instance.DisplayName;
        roomPlayerLobby.SetName(PlayerNameInput.Instance.DisplayName);
        Debug.Log(DisplayName,this.gameObject);
        roomPlayerLobby.transform.SetParent(GameObject.Find("Parent").transform);
    }
    
}


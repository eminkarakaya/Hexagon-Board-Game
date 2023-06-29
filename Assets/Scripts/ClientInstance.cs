using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ClientInstance : NetworkBehaviour
{
    public static ClientInstance instance;
    [SerializeField] private NetworkIdentity _playerPrefab = null;
    private void NetworkSpawnPlayer()
    {
        GameObject go = Instantiate(_playerPrefab.gameObject,transform.position,Quaternion.identity);
        NetworkServer.Spawn(go,base.connectionToClient);   
    }
    public static ClientInstance ReturnClientInstance(NetworkConnection conn = null)
    {
        if(NetworkServer.active && conn != null)
        {
            NetworkIdentity localPlayer;
            if(NetworkManagerGdd.LocalPlayers.TryGetValue(conn,out localPlayer))
                return localPlayer.GetComponent<ClientInstance>();
            else
                return null;
        }
        else   
            return instance;
    }
}

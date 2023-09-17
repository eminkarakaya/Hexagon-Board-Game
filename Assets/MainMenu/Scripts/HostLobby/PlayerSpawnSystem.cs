using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    public override void OnStartServer() => NetworkManagerGdd.OnServerReadied += SpawnPlayer;

   

    [ServerCallback]
    private void OnDestroy() => NetworkManagerGdd.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {

        GameObject playerInstance = Instantiate(playerPrefab);
        NetworkServer.Spawn(playerInstance, conn);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkManagerGdd : NetworkManager
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        // Debug.Log(connectionToClient);
    }
}

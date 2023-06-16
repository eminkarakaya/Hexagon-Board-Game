using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class RoomSettings : SingletonMirror<RoomSettings>
{
    [SyncVar] public List<PlayerMainMenu> allClients = new List<PlayerMainMenu>();
}

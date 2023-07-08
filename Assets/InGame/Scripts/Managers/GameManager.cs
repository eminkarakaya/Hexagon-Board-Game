using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    public static GameManager singleton;
     public Transform civUIParent;
    public  List<Hex > npcHexes;
    public  List<Hex > playersHexes;
    public  List<Building> buildings;

    // Overrides the base singleton so we don't
    // have to cast to this type everywhere.
    public List<AIManager> npcs = new List<AIManager>();
    
    public static Dictionary<NetworkConnection,PlayerManager> LocalPlayers = new Dictionary<NetworkConnection, PlayerManager>();
    private void Awake() {
        singleton = this;
    }
}

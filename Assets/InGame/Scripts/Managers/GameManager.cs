using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public Button OrderButton;
    public static GameManager singleton;
    public int playerCount;
     public Transform civUIParent;
    public  List<Hex > npcHexes;
    public  List<Hex > playersHexes;
    public  List<Building> buildings;
    [SerializeField] private int count;

    // Overrides the base singleton so we don't
    // have to cast to this type everywhere.
    public List<AIManager> npcs = new List<AIManager>();
    
    private void Awake() {
        singleton = this;
    }

}

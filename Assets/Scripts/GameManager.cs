using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : SingletonMirror<GameManager>
{
    [SyncVar] public  List<GameObject> buildingGO;
     public List<Hex > hexes;
    public string NickName;
    [SyncVar]public List<Building> buildings;
    
    [SyncVar] [SerializeField] public List<int> side = new List<int>(){1,2};
       
    
}

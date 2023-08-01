using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public TMP_Text goldText,goldPerRoundText;
    public TMP_Text nextRoundTipText;
    public HoverTip hoverTip;
    public Button OrderButton;
    public static GameManager instance;
    public int playerCount;
     public Transform civUIParent;
    public  List<Hex > npcHexes;
    public  List<Hex > playersHexes;
    public  List<Building> buildings;
    [SerializeField] private int count;
    public PlayerManager ownedPlayerManager;
    [SyncVar] public int hexIndex;

    // Overrides the base singleton so we don't
    // have to cast to this type everywhere.
    public List<AIManager> npcs = new List<AIManager>();

    public List<CivData> allCivData;
    
    private void Awake() {
        instance = this;
    }

    public CivData GetCivData(int index)
    {
        return allCivData[index];
    }
    public int GetIndexCivData(CivData data)
    {
        for (int i = 0; i < allCivData.Count; i++)
        {
            if(allCivData[i] == data)
                return i;
        }
        return -1;
    }
    // public static IEnumerator wait(CivManager attackableCivManager, CivManager thisCivManager,System.Action CMDSetSide,GameObject gameObject, bool isOwned)
    // {
    //     thisCivManager.CMDRemoveOwnedObject(gameObject); //requestauthority = false
    //     thisCivManager = attackableCivManager;
    //     while(isOwned == false)
    //     {
    //         yield return null;
    //     }
        
    //     // ele gecırıldıkten sonra
    //     CMDSetSide?.Invoke();
    //     attackableCivManager.CMDShowAllUnits();
    //     attackableCivManager.CMDAddOwnedObject(gameObject); //requestauthority = false
    // }
    // public static void CMDSetSide(CivManager civManager,ISideable sideable)
    // {
    //     RPGSetSide(civManager,sideable);
        
    // }
    // private static void RPGSetSide(CivManager civManager,ISideable sideable)
    // {
    //     if(civManager.isOwned)
    //     {
    //         SetSide(Side.Me,sideable.Outline,sideable);
    //     }
    //     else
    //     {
    //         SetSide(Side.Enemy,sideable.Outline,sideable);
    //     }
    // }
    // public static void SetSide(Side side, Outline outline,ISideable sideable)
    // {
    //     sideable.Side = side;
    //     if(outline == null) return;
    //     if(side == Side.Me)
    //     {
    //         outline.OutlineColor = Color.white;
    //     }
    //     else if(side == Side.Enemy)
    //     {
    //         outline.OutlineColor = Color.red;
    //     }
    //     else
    //         outline.OutlineColor = Color.blue;

    // }

    

}

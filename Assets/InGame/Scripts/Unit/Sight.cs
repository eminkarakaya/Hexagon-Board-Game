using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Sight : NetworkBehaviour
{
    public ISightable sightable;
    private HexGrid hexGrid;

    SightResult sightRange;
    [SerializeField] private int sightDistance = 2;
    public int SightDistance {get => sightDistance;}
    public override void OnStartClient()
    {
        sightable = GetComponent<ISightable>();
    }
    private void Awake() {
        
    }
    public void ShowSight(Hex hex)
    {
        if(hex.IsEnemy() ||hex.IsEnemyBuilding()|| hex.IsEnemySettler()) return;
        hexGrid = FindObjectOfType<HexGrid>();
        // HideSight(hex);
        sightRange = GraphSearch.GetRangeSightDistance(hex.HexCoordinates,sightDistance,hexGrid);
        foreach (var item in sightRange.GetRangeSight())
        {
            if(hexGrid.GetTileAt(item) != null)
            {
                Hex hex1 = hexGrid.GetTileAt(item);
                hex1.transform.GetChild(2).gameObject.SetActive(false);   
                hex1.OpenLinkedObjectSight();
                hex1.isVisible=true;
            }
        }
    }

    public void HideSight(Hex hex)
    {
        hexGrid = FindObjectOfType<HexGrid>();
        if(sightRange.sightNodesDict == null) 
        {
            if(sightable == null)
            {
                sightable = GetComponent<ISightable>();
            }
            sightRange = GraphSearch.GetRangeSightDistance(hex.HexCoordinates,sightDistance,hexGrid);
        }
        
        // if(Side == Side.Enemy) return;
        foreach (var item in sightRange.sightNodesDict)
        {
            Hex [] hexes = FindObjectsOfType<Hex>();
            Hex hex1 = hexGrid.GetTileAt(item.Key);
            hex1.transform.GetChild(2).gameObject.SetActive(true);   
            hex1.CloseLinkedObjectSight();
            hex.isVisible = false;
        }
    }
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Sight : NetworkBehaviour
{
    public IMovable movable;
    Side side;
    private HexGrid hexGrid;
    [SerializeField] private List<GameObject> _sight;
    public List<GameObject> sight{get => _sight;}

    SightResult sightRange;
    [SerializeField] private int sightDistance = 2;
    public int SightDistance {get => sightDistance;}
    private void Awake() {
        movable = GetComponent<IMovable>();
    }
    public void ShowSight(Hex hex)
    {
        if(side == Side.Enemy) return;
        hexGrid = FindObjectOfType<HexGrid>();
        // HideSight(hex);
        sightRange = GraphSearch.GetRangeSightDistance(hex.HexCoordinates,sightDistance,hexGrid);
        foreach (var item in sightRange.GetRangeSight())
        {
            if(hexGrid.GetTileAt(item) != null)
            {
                Hex hex1 = hexGrid.GetTileAt(item);
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
            sightRange = GraphSearch.GetRangeSightDistance(hex.HexCoordinates,sightDistance,hexGrid);
        }
        // if(Side == Side.Enemy) return;
        foreach (var item in sightRange.sightNodesDict)
        {
            Hex [] hexes = FindObjectsOfType<Hex>();
           
            Hex hex1 = hexGrid.GetTileAt(item.Key);
            hex1.CloseLinkedObjectSight();
            hex.isVisible = false;
        }
    }
   
}

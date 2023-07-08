using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Vision : NetworkBehaviour
{
    public IVisionable visionable;
    private HexGrid hexGrid;

    VisionResult visionRange;
    [SerializeField] private int visionDistance = 2;
    public int VisionDistance {get => visionDistance;}
    public override void OnStartClient()
    {
        visionable = GetComponent<IVisionable>();
    }
    private void Awake() {
        
    }
    public void ShowVision(Hex hex)
    {
        if(hex.IsEnemy() ||hex.IsEnemyBuilding()|| hex.IsEnemySettler()) return;
        hexGrid = FindObjectOfType<HexGrid>();
        visionRange = GraphSearch.GetRangeVisionDistance(hex.HexCoordinates,visionDistance,hexGrid);
        foreach (var item in visionRange.GetRangeVision())
        {
            if(hexGrid.GetTileAt(item) != null)
            {
                Hex hex1 = hexGrid.GetTileAt(item);
                hex1.transform.GetChild(2).gameObject.SetActive(false);   
                hex1.OpenLinkedObjectVision();
                hex1.isVisible=true;
            }
        }
    }

    public void HideVision(Hex hex)
    {
        hexGrid = FindObjectOfType<HexGrid>();
        if(visionRange.visionNodesDict == null) 
        {
            if(visionable == null)
            {
                visionable = GetComponent<IVisionable>();
            }
            visionRange = GraphSearch.GetRangeVisionDistance(hex.HexCoordinates,visionDistance,hexGrid);
        }
        
        // if(Side == Side.Enemy) return;
        foreach (var item in visionRange.visionNodesDict)
        {
            Hex [] hexes = FindObjectsOfType<Hex>();
            Hex hex1 = hexGrid.GetTileAt(item.Key);
            hex1.transform.GetChild(2).gameObject.SetActive(true);   
            hex1.CloseLinkedObjectVision();
            hex.isVisible = false;
        }
    }
   
}

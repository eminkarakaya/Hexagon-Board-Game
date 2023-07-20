using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
[System.Serializable]
public struct ResourceBtn
{
    public ResourceType resourceType;
    public Button button;
}
public class HarborSettler : Settler
{
    
    [SerializeField] private List<ResourceBtn> resourceBtns = new List<ResourceBtn>();
    [Command] public override void CMDCreateBuilding()
    {
        if(Hex.Building != null) return;
        // if(!Hex.isCoast) return;
        Building unit = Instantiate(Hex.resource.prefab).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateBuilding(unit);
        DeselectSettler();
    }
    public override void SelectSettler()
    {
        HexGrid hexGrid = FindObjectOfType<HexGrid>();
        foreach (var item in hexGrid.GetHarborHex())
        {  
            item.EnableHighligh(); 
        }
    }
    public override void DeselectSettler()
    {
        base.DeselectSettler();
        HexGrid hexGrid = FindObjectOfType<HexGrid>();
        foreach (var item in hexGrid.GetHarborHex())
        {  
            item.DisableHighligh(); 
        }
    }
    protected override void ToggleButtonsVirtual(bool state)
    {
        if(state == false)
        {
            foreach (var item in resourceBtns)
            {
                item.button.interactable = false;
            }
        }
        else
        {
            ToggleResourceBtn();
        }
    }
    public void ToggleResourceBtn()
    {
        foreach (var item in resourceBtns)
        {
            if(item.resourceType == Hex.resource.ResourceType)
            {
                item.button.interactable = true;
            }
            else
                item.button.interactable = false;
        }
    }
   
}

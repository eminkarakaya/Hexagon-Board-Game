using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

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
    [Command] public void CMDCreateMine()
    {
        if(Hex.resource.mine != null) return;
        // if(!Hex.isCoast) return;
        Mine mine = Instantiate(Hex.resource.prefab).GetComponent<Mine>();
        NetworkServer.Spawn(mine.gameObject,connectionToClient);
        mine.Hex = Hex;
        RPCCreateMine(mine);
        DeselectSettler();
    }
    [ClientRpc] public void RPCCreateMine(Mine mine)
    {
        mine.transform.position = new Vector3 (Hex.transform.position.x , 1 , Hex.transform.position.z );
        mine.transform.rotation = Quaternion.Euler(-90,0,0);
        mine.Hex = Hex;
        mine.Hex.resource.mine = mine;
        mine.CivManager = CivManager;
        CivManager.CMDAddOwnedObject(mine.gameObject);
        var mines = FindObjectsOfType<Mine>().ToList();
        foreach (var item in mines)
        {
            if(item == null) continue;
            if(item.isOwned)
            {
                item.SetSide(Side.Me,item.GetComponent<Outline>());
            }
            else
                item.SetSide(Side.Enemy,item.GetComponent<Outline>());
        }
        CivManager.SetTeamColor(mine.gameObject);
        Result.HideRange(this,Movement);  
        UnitManager.Instance.selectedUnit = null;
        CivManager.DestroyObj(this.gameObject);
        mine.InitializeMine();
    }
    public void CreateMineOnClick()
    {
        CMDCreateMine();
        TaskComplate();
        CivManager.CMDRemoveOwnedObject(this.gameObject);
    }
}

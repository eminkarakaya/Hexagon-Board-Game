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
public class Worker : Settler
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
        Result.HideRange(this,Movement);  
        UnitManager.Instance.selectedUnit = null;
        NetworkServer.Destroy(this.gameObject);
    }
    private void OnDisable() {
        if(isOwned)
        Result.HideRange(this,Movement);
    }
    public override void SelectSettler()
    {
        // HexGrid hexGrid = FindObjectOfType<HexGrid>();
        // foreach (var item in hexGrid.GetHarborHex())
        // {  
        //     item.EnableHighligh(); 
        // }
    }
    // public override void DeselectSettler()
    // {
        // base.DeselectSettler();
        // HexGrid hexGrid = FindObjectOfType<HexGrid>();
        // foreach (var item in hexGrid.GetHarborHex())
        // {  
        //     item.DisableHighligh(); 
        // }
    // }
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
        
        mine.SetSide(this.Side,mine.GetComponent<OutlineObj>());
        CivManager.CMDSetTeamColor(mine.gameObject);
        Result.HideRange(this,Movement);  
        UnitManager.Instance.selectedUnit = null;
        NetworkServer.Destroy(this.gameObject);
        mine.InitializeMine();
    }
    public void CreateMineOnClick()
    {
        ToopltipManager.Hide();
        
        CMDCreateMine();
        TaskComplate();
        CivManager.CMDRemoveOwnedObject(this.gameObject);

        UnitManager.Instance.ClearOldSelection();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingManager : MonoBehaviour


{
    
    [SerializeField] private Building selectedBuilding;
    
    [SerializeField] private UnityEvent OnSelectedBuilding;
    public void HandleUnitSelected(GameObject building)
    {
        
        Building buildingReference = building.GetComponent<Building>();
        if(buildingReference.Side == Side.Enemy)
        {
            return;
        }
        if(CheckIfTheSameUnitSelected(buildingReference)) 
            return;
        
        selectedBuilding = buildingReference;   
        buildingReference.OpenCanvas();
        OnSelectedBuilding?.Invoke();
        
    }
    private bool CheckIfTheSameUnitSelected(Building buildingReference)
    {
        if (this.selectedBuilding == buildingReference)
        {
            ClearOldSelection();
            buildingReference.CloseCanvas();
            return true;
        }
        return false;
    }
    private void ClearOldSelection()
    {
        if(selectedBuilding == null) return;

        selectedBuilding.CloseCanvas();
        this.selectedBuilding = null;
    }
    public void ResetBuildingSelection()
    {
        ClearOldSelection();
    }
}

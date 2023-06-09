using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    public LayerMask selectionMask;
    
    public UnityEvent<GameObject> OnUnitSelected,TerrainSelected,OnUnitSelectedRightClick,OnTerrainSelectedRightClick,OnBuildingSelected;
    private void Awake() {
        if(_mainCamera == null) _mainCamera = Camera.main;
    }
    
    public void HandleClick(Vector3 mousePosition)
    {
        GameObject result;
        if(FindTarget(mousePosition,out result))
        {
            if(UnitSelected(result))
            {
                OnUnitSelected?.Invoke(result);
            }
            else if(BuildingSelected(result))
            {
                OnBuildingSelected?.Invoke(result);
            }
            else
                TerrainSelected?.Invoke(result);

        }
    }
    public void HandleRightClick(Vector3 mousePosition)
    {
        GameObject result;
        if(FindTarget(mousePosition,out result))
        {
            if(UnitSelected(result))
            {
                OnUnitSelectedRightClick?.Invoke(result);
            }
            else
                OnTerrainSelectedRightClick?.Invoke(result);
        }
    }
    private bool UnitSelected(GameObject result)
    {
        return result.GetComponent<Unit>() != null;
    }
    private bool BuildingSelected(GameObject result)
    {
        return result.GetComponent<Building>() != null;
    }
    
    private bool FindTarget(Vector3 mousePosition,out GameObject result)
    {
        RaycastHit hit;
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
        if(Physics.Raycast(ray,out hit,10000f,selectionMask ))
        {
            result = hit.collider.gameObject;
            return true;
        }
        result = null;
        return false;
    }
}

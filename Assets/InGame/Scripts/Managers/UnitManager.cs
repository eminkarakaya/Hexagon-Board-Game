using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Mirror;
public class UnitManager : SingletonMirror<UnitManager>
{
    [SerializeField]
    private HexGrid hexGrid;
    public LayerMask layer,defaulLayer;
    [SerializeField] private UnityEvent OnSelectedUnit;
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private bool playersTurn = true;
    public bool PlayersTurn { get => playersTurn; private set{} }


    [SerializeField]
    public ISelectable selectedUnit;
    private Hex previouslySelectedHex;

    public void HandleESC()
    {
        if(selectedUnit != null)
        {
            selectedUnit.CloseCanvas();
        }
    }

    public void HandleUnitSelected(Transform unit)
    {
        if (PlayersTurn == false)
            return;
        ISelectable selectableReference = unit.GetComponent<ISelectable>();
        if(selectableReference.Side == Side.Enemy)
        {
            return;
        }
       
        if (CheckIfTheSameUnitSelected(selectableReference))
            return;

        OnSelectedUnit?.Invoke();

        PrepareUnitForMovement(selectableReference);
    }
    public void ResetSelectedUnit()
    {
        ClearOldSelection();
    }
    public void HandleEnemyUnitSelected(ISelectable selectable)
    {
        
    }
    private bool CheckIfTheSameUnitSelected(ISelectable selectableReference)
    {
        if ((object)this.selectedUnit == selectableReference)
        {
            ClearOldSelection();
            selectableReference.CloseCanvas();
            return true;
        }
        return false;
    }

    public void HandleTerrainSelected(GameObject hexGO) // terraine sol clıck
    {
        if (selectedUnit == null || PlayersTurn == false)
        {
            return;
        }
        // ClearOldSelection();
    }
    public void HandleTerrainSelectedRightClick(GameObject hexGO) // move
    {
        if(selectedUnit == null || PlayersTurn == false)
            return;
        Hex selectedHex = hexGO.GetComponent<Hex>();
        if(selectedHex.isReachable == false) return;
        if (/*HandleHexOutOfRange(selectedHex.HexCoordinates) ||*/ HandleSelectedHexIsUnitHex(selectedHex.HexCoordinates))
            return;

        HandleTargetHexSelected(selectedHex);
    }

    public void HandleUnitSelectedRightClick(GameObject gameObject)
    {
        if(selectedUnit == null || PlayersTurn == false)
            return;
        Hex selectedHex = gameObject.GetComponent<ISelectable>().Hex;
        if (/*HandleHexOutOfRange(selectedHex.HexCoordinates) ||*/ HandleSelectedHexIsUnitHex(selectedHex.HexCoordinates))
            return;
        HandleTargetHexSelected(selectedHex);
    }
    private void PrepareUnitForMovement(ISelectable selectableReference)
    {
        if (this.selectedUnit != null)
        {
            ClearOldSelection();
        }

        this.selectedUnit = selectableReference;
        selectableReference.OpenCanvas();
        selectedUnit.LeftClick();
    }

    public void ClearOldSelection()
    {
        if(selectedUnit == null) return;
        selectedUnit.CloseCanvas();
        previouslySelectedHex = null;
        this.selectedUnit.Deselect();
        this.selectedUnit = null;

    }

    private void HandleTargetHexSelected(Hex selectedHex)
    {
        previouslySelectedHex = selectedHex;
        selectedUnit.RightClick(selectedHex);
        selectedUnit.RightClick2(selectedHex);
        ClearOldSelection();
    }

    private bool HandleSelectedHexIsUnitHex(Vector3Int hexPosition)
    {
        if (hexPosition == hexGrid.GetClosestHex(selectedUnit.Hex.transform.position))
        {
            ClearOldSelection();
            return true;
        }
        return false;
    }


    private void ResetTurn(Movement selectedUnit)
    {
        // selectedUnit.MovementFinished -= ResetTurn;
        PlayersTurn = true;
    }
}
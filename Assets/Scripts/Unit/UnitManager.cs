using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class UnitManager : SingletonMirror<UnitManager>
{
    [SerializeField]
    private HexGrid hexGrid;
    public LayerMask layer,defaulLayer;
    [SerializeField] private UnityEvent OnSelectedUnit;
    [SerializeField] private MovementSystem movementSystem;
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private bool playersTurn = true;
    public bool PlayersTurn { get => playersTurn; private set{} } 
    

    [SerializeField]
    public Unit selectedUnit;
    private Hex previouslySelectedHex;
    

    public void HandleUnitSelected(GameObject unit)
    {
        if (PlayersTurn == false)
            return;
        Unit unitReference = unit.GetComponent<Unit>();
        if(unitReference.Side == Side.Enemy)
        {
            return;
        }   
        else
        {
            
        }
        if (CheckIfTheSameUnitSelected(unitReference))
            return;

        OnSelectedUnit?.Invoke();
        
        PrepareUnitForMovement(unitReference);
    }
    public void ResetSelectedUnit()
    {
        ClearOldSelection();
    }
    public void HandleEnemyUnitSelected(GameObject unit)
    {
        if(selectedUnit != null)
        {
        }
    }
    private bool CheckIfTheSameUnitSelected(Unit unitReference)
    {
        if (this.selectedUnit == unitReference)
        {
            ClearOldSelection();
            unitReference.CloseCanvas();
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
        ClearOldSelection();
    }
    public void HandleTerrainSelectedRightClick(GameObject hexGO) // move
    {
        if(selectedUnit == null || PlayersTurn == false)
            return;
        Hex selectedHex = hexGO.GetComponent<Hex>();
        if(attackSystem.CheckEnemyInRange(selectedHex))
        {
            if(selectedHex.Building != null)
            {
                selectedUnit.GetComponent<Attack>().AttackUnit(selectedHex.Building.hp);
            }
            else
                selectedUnit.GetComponent<Attack>().AttackUnit(selectedHex.Unit.hp);
            // selectedUnit.SetCurrentMovementPoints(0);
            ClearOldSelection();
            return;
        }
        if (HandleHexOutOfRange(selectedHex.HexCoordinates) || HandleSelectedHexIsUnitHex(selectedHex.HexCoordinates))
            return;
        
        HandleTargetHexSelected(selectedHex);
    }

    public void HandleUnitSelectedRightClick(GameObject unit)  
    {
        // if(selectedUnit == null) // eger secılı unıt yoksa return
        // {
        //     return;
        // }
        // Hex selectedHex = selectedUnit.Hex;
        // if (HandleHexOutOfRange(selectedHex.HexCoordinates) || HandleSelectedHexIsUnitHex(selectedHex.HexCoordinates))
        //     return;
        // HandleTargetHexSelected(hexGrid.GetTileAt(GraphSearch.GetCloseseteHex(movementSystem.movementRange.allNodesDict,selectedHex.HexCoordinates)));
    }
    private void PrepareUnitForMovement(Unit unitReference)
    {
        if (this.selectedUnit != null)
        {
            ClearOldSelection();
        }

        this.selectedUnit = unitReference;
        this.selectedUnit.Select();
        unitReference.OpenCanvas();
        movementSystem.ShowRange(this.selectedUnit);
        attackSystem.ShowRange(selectedUnit);
    }

    private void ClearOldSelection()
    {
        if(selectedUnit == null) return;
        selectedUnit.CloseCanvas();
        previouslySelectedHex = null;
        this.selectedUnit.Deselect();
        movementSystem.HideRange();
        attackSystem.HideRange();
        this.selectedUnit = null;

    }

    private void HandleTargetHexSelected(Hex selectedHex)
    {   
        if (previouslySelectedHex == null || previouslySelectedHex != selectedHex)
        {
            previouslySelectedHex = selectedHex;
            movementSystem.ShowPath(selectedHex.HexCoordinates, this.hexGrid,selectedUnit.GetComponent<Attack>().range);
        }
        else
        {
            // if(selectedUnit.Hex.IsEnemy())
            // {
            //     movementSystem.MoveUnit(selectedUnit, this.hexGrid,selectedHex);
            // }
            // selectedUnit.Hex.Unit = null;
           
                movementSystem.MoveUnit(selectedUnit, this.hexGrid,selectedHex,selectedUnit.GetComponent<Attack>().range);

            
            PlayersTurn = false;
            selectedUnit.MovementFinished += ResetTurn;
            ClearOldSelection();
        }
    }

    private bool HandleSelectedHexIsUnitHex(Vector3Int hexPosition)
    {
        if (hexPosition == hexGrid.GetClosestHex(selectedUnit.transform.position))
        {
            selectedUnit.Deselect();
            ClearOldSelection();
            return true;
        }
        return false;
    }

    private bool HandleHexOutOfRange(Vector3Int hexPosition)
    {
        if(!hexGrid.GetTileAt(hexPosition).isVisible) return false;
        if (movementSystem.IsHexInRange(hexPosition) == false)
        {
            return true;
        }
        return false;
    }

    private void ResetTurn(Unit selectedUnit)
    {
        selectedUnit.MovementFinished -= ResetTurn;
        PlayersTurn = true;
    }
}
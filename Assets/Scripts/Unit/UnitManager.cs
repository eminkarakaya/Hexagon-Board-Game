using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private HexGrid hexGrid;

    [SerializeField] private MovementSystem movementSystem;
    [SerializeField] private AttackSystem attackSystem;
    [SerializeField] private bool playersTurn = true;
    public bool PlayersTurn { get => playersTurn; private set{} } 

    [SerializeField]
    private Unit selectedUnit;
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

        PrepareUnitForMovement(unitReference);
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
            selectedUnit.GetComponent<Attack>().AttackUnit(selectedHex.Unit);
            selectedUnit.SetCurrentMovementPoints(0);
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
        
        movementSystem.ShowRange(this.selectedUnit, this.hexGrid);
        attackSystem.ShowRange(selectedUnit);
    }

    private void ClearOldSelection()
    {
        previouslySelectedHex = null;
        this.selectedUnit.Deselect();
        movementSystem.HideRange(this.hexGrid);
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
        if (movementSystem.IsHexInRange(hexPosition) == false)
        {
            // Debug.Log("Hex Out of range!");
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
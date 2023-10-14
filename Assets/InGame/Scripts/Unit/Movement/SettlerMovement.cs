using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SettlerMovement : Movement
{ 
    private void OnDrawGizmos() {
        movementSystem = InitMovementSystem();
        if(movementSystem.movementRange.visitedNodesDict== null || !isOwned)
        {
            return;
        }
        HexGrid hexGrid = FindObjectOfType<HexGrid>();
        foreach (var item in movementSystem.movementRange.visitedNodesDict )
        {
            Vector3 startPos =hexGrid.GetTileAt (item.Key).transform.position;
            if( item.Value != null)
            {
                Vector3 valuePos = hexGrid.GetTileAt ((Vector3Int)item.Value).transform.position;
                DrawArrow.ForGizmo(valuePos + Vector3.up * h,(startPos-valuePos),Color.red,.5f,25);
            }
        }
    }
    protected override IEnumerator MovementCoroutine(Vector3 endPos,Hex endHex,Hex lastHex,MovementSystem movementSystem)
    {
        animator.SetBool("Move",true);
        Moveable.ToggleButtons(false);
        Vector3 startPos = transform.position;
        endPos.y = startPos.y;
        if(endHex.IsEnemy() || endHex.IsEnemySettler() || endHex.IsEnemyBuilding())
        {
            yield break;
        }
        // Movementstart
        float timeElapsed = 0f;
        while(timeElapsed<movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration;
            transform.position = Vector3.Lerp(startPos,endPos,lerpStep);
            yield return null;
        }
        // MovementFinish
        
        // MovementFinishEvents
        Moveable.CivManager.CMDHideAllUnits();
        
        CMDHide();
        CMDSetHex(endHex,Moveable.Hex);
        this.Moveable.Hex = endHex;

        transform.position = endPos;
        Moveable.CivManager.CMDShowAllUnits();
        CMDShow();
        CurrentMovementPoints -= 1;
        
        if(pathPositions.Count > 0)
        {
            animator.SetBool("Move",false);
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(),pathHexes.Dequeue(),lastHex,rotationDuration,movementSystem));
        }
        else
        {
            
            MovementFinsihEvent(this);
            if(lastHex.Settler != null && lastHex.Settler.Side == Side.Enemy)
            {
                Quaternion startRotation = transform.rotation;
                Vector3 direction = new Vector3(lastHex.transform.position.x,transform.position.y,lastHex.transform.position.z) - transform.position;
                Quaternion endRotation = Quaternion.LookRotation(direction,Vector3.up);
                timeElapsed = 0;
                while(timeElapsed < rotationDuration)
                {
                    timeElapsed += Time.deltaTime;
                    float lerpStep = timeElapsed / rotationDuration;
                    transform.rotation = Quaternion.Lerp(startRotation,endRotation,lerpStep);
                    yield return null;
                }
                transform.rotation = endRotation;
            }
        }
        animator.SetBool("Move",false);
        Moveable.ToggleButtons(true);
        
    }
     [Command]
    protected override void CMDShow()
    {
        RPCShow();
    }
    [ClientRpc]
    protected void RPCShow()
    {
        movementSystem = InitMovementSystem();
        if(UnitManager.Instance.selectedUnit != null && UnitManager.Instance.SelectedMoveable != null)
        {
            movementSystem.ShowRange(UnitManager.Instance.SelectedMoveable,UnitManager.Instance.SelectedMoveable.Movement);
        }
    }
    [ClientRpc]
    protected void RPCHide()
    {
        movementSystem = InitMovementSystem();
        if(UnitManager.Instance.selectedUnit != null && UnitManager.Instance.SelectedMoveable != null)
        {
            movementSystem.HideRange(UnitManager.Instance.SelectedMoveable,UnitManager.Instance.SelectedMoveable.Movement);
        }
        
    }
    [Command]
    protected override void CMDHide()
    {
       
       RPCHide();
    }

    protected override MovementSystem InitMovementSystem()
    {
        return new SettlerMovementSystem(Moveable);
    }
}

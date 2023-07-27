using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SettlerMovement : Movement
{ 
    
    protected override IEnumerator MovementCoroutine(Vector3 endPos,Hex endHex,Hex hex,MovementSystem movementSystem)
    {
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
        playerManager.CMDHideAllUnits();
        
        CMDHide();
        CMDSetHex(endHex,Moveable.Hex);
        this.Moveable.Hex = endHex;

        transform.position = endPos;
        playerManager.CMDShowAllUnits();
        CMDShow();
        CurrentMovementPoints -= 1;
        
        if(pathPositions.Count > 0)
        {
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(),pathHexes.Dequeue(),hex,rotationDuration,movementSystem));
        }
        else
        {
            
            MovementFinsihEvent(this);
            if(hex.Settler != null && hex.Settler.Side == Side.Enemy)
            {
                Quaternion startRotation = transform.rotation;
                Vector3 direction = new Vector3(hex.transform.position.x,transform.position.y,hex.transform.position.z) - transform.position;
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

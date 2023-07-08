using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class UnitMovement : Movement
{
    MovementSystem movementSystem;
    
    protected override IEnumerator MovementCoroutine(Vector3 endPos,Hex nextHex,Hex lastHex,MovementSystem movementSystem)
    {
        Vector3 startPos = transform.position;
        endPos.y = startPos.y;
        if(lastHex != nextHex && nextHex.IsEnemySettler())
            yield break;
        // if(lastHex != nextHex && nextHex.IsEnemyBuilding())
        //     yield break;
        
        if(nextHex.IsEnemy() || nextHex.IsEnemyBuilding())
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
        CMDSetHex(nextHex,Moveable.Hex);
        this.Moveable.Hex = nextHex;

        transform.position = endPos;
        playerManager.CMDShowAllUnits();
        CMDShow();
        
        if(pathPositions.Count > 0)
        {
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(),pathHexes.Dequeue(),lastHex,rotationDuration,movementSystem));
        }
        else
        {  
            MovementFinsihEvent(this);
            if(lastHex.IsEnemy() ||lastHex.IsEnemyBuilding())
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
                AttackUnit(lastHex);
            }
            else if(lastHex.IsEnemySettler())
            {
                GetComponent<Unit>().CivManager.Capture(lastHex.Settler.GetComponent<NetworkIdentity>());     
                lastHex.Settler.StartCoroutine1(lastHex.Settler.GetComponent<NetworkIdentity>(),lastHex.Settler.gameObject,GetComponent<Unit>().CivManager);

            }
        }
    }
    
    public IEnumerator MoveKill(Hex hex,bool state)
    {
        if(state)
        {
            playerManager.CMDHideAllUnits();
            
            CMDHide();
            CMDSetHex(hex,Moveable.Hex);
            this.Moveable.Hex = hex;

            transform.position = new Vector3(hex.transform.position.x , 1 , hex.transform.position.z);
            playerManager.CMDShowAllUnits();
            CMDShow();
            Vector3 startPos = transform.position;
            float timeElapsed = 0f;
            while(timeElapsed<movementDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / movementDuration;
                transform.position = Vector3.Lerp(startPos,new Vector3(hex.transform.position.x , 1 , hex.transform.position.z),lerpStep);
                yield return null;
            }
        }
    }


    [Command]
    protected  override void CMDShow()
    {
        RPCShow();
    }
    [ClientRpc]
    protected void RPCShow()
    {
        movementSystem = new UnitMovableResult(Moveable);
        if(UnitManager.Instance.selectedUnit != null && UnitManager.Instance.selectedUnit.Movable != null)
        {
            movementSystem.ShowRange(UnitManager.Instance.selectedUnit.Movable,UnitManager.Instance.selectedUnit.Movable.Movement);
        }
       
    }
    [ClientRpc]
    protected void RPCHide()
    {
        movementSystem = new UnitMovableResult(Moveable);
        if(UnitManager.Instance.selectedUnit != null && UnitManager.Instance.selectedUnit.Movable != null)
        {
            movementSystem.HideRange(UnitManager.Instance.selectedUnit.Movable,UnitManager.Instance.selectedUnit.Movable.Movement);
        }
        
    }
    [Command]
    protected  override void CMDHide()
    {
       RPCHide();
    }
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerMovement : Movement
{
    protected override IEnumerator MovementCoroutine(Vector3 endPos,Hex endHex,Hex hex,MovementSystem movementSystem)
    {
        Vector3 startPos = transform.position;
        endPos.y = startPos.y;
        if(endHex.IsEnemy())
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
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.CMDHideAllUnits();
        
        CMDHide(this);
        CMDSetHex(endHex,Moveable.Hex);
        this.Moveable.Hex = endHex;

        transform.position = endPos;
        playerManager.CMDShowAllUnits();
        CMDShow();
        
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
    }
}

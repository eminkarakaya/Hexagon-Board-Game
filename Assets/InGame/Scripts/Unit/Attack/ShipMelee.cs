using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMelee : Melee
{
    [SerializeField] float _attackTime;
    public override IEnumerator AttackUnit(IDamagable damagable,IAttackable attackable,float movementDuration)
    {
        if(!damagable.Hex.IsWater()  && !damagable.Hex.IsBuilding()) yield break;
        attackTime = _attackTime;
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) yield break;
        // if(damagable.Hex.IsWater()) yield break;
        float timeElapsed = 0f;
        while(timeElapsed<movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration;
            transform.position = Vector3.Lerp(transform.position,TransformUtils.FixY(damagable.Hex.transform.position),lerpStep);
            yield return null;
        }   

        StartCoroutine(OpenTransparentMaterial(attackTime/2));
        timeElapsed = 0f;
        while(timeElapsed<attackTime )
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        int hp  = damagable.hp.Hp;
        
                StartCoroutine(CloseTransparentMaterial(attackTime/2));
        if(isServer)
        {
            Attack(damagable.hp);            
        }
        else
        {
            CMDAttack(damagable.hp);
        }
        bool kill = true;
        while(hp == damagable.hp.Hp)
        {
            yield return null;
        }
        // kill events
        foreach (var item in PropertiesEnumList)
        {

            if(item.attackPropertiesEnum == PropertiesEnum.Default)
            {
                if(TryGetComponent(out UnitMovement movement0))
                {
                    if(damagable.hp.Hp<=0)
                        StartCoroutine (movement0.MoveKill(damagable.Hex,damagable.hp.Hp<=0));
                    else
                    {
                            timeElapsed = 0f;
                        while(timeElapsed<movementDuration)
                        {
                            timeElapsed += Time.deltaTime;
                            float lerpStep = timeElapsed / movementDuration;
                            transform.position = Vector3.Lerp(transform.position,TransformUtils.FixY(attackable.Hex.transform.position),lerpStep);
                            yield return null;
                        }
                    }
                }
            }
            if(item.attackPropertiesEnum == PropertiesEnum.TakeHostage)
            {
                if(TryGetComponent(out UnitMovement movement1))
                {
                    if(damagable.hp.Hp<=0)
                    {
                        movement1.TakeHostage(damagable.Hex,damagable.hp.Hp<=0);
                        kill = false;
                    }
                    else
                    {
                            timeElapsed = 0f;
                        while(timeElapsed<movementDuration)
                        {
                            timeElapsed += Time.deltaTime;
                            float lerpStep = timeElapsed / movementDuration;
                            transform.position = Vector3.Lerp(transform.position,TransformUtils.FixY(attackable.Hex.transform.position),lerpStep);
                            yield return null;
                        }
                    }
                }
            }
            
        }
         
        damagable.hp.Death(damagable,attackable,kill,killEvent);
        
            
    }
}

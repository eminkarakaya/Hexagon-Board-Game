using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMelee : Melee
{
    public override IEnumerator AttackUnit(IDamagable damagable,IAttackable attackable,float movementDuration)
    {
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) yield break;
        if(!damagable.Hex.IsWater()  && !damagable.Hex.IsBuilding()) yield break;
        if(isServer)
        {
            Attack(damagable.hp);            
        }
        else
        {
            CMDAttack(damagable.hp);
        }
        if(TryGetComponent(out UnitMovement movement))
        {
            StartCoroutine(movement.MoveKill(damagable.Hex,damagable.hp.Hp<=0));
        }
        damagable.hp.Death(damagable,attackable);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMelee : Melee
{
    public override void AttackUnit(IDamagable damagable,IAttackable attackable)
    {
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) return;
        if(!damagable.Hex.IsWater()  && !damagable.Hex.IsBuilding()) return;
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

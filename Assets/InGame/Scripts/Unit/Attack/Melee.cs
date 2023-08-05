using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class Melee : Attack
{
    
    public override void AttackUnit(IDamagable damagable,IAttackable attackable)
    {
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) return;
        if(damagable.Hex.IsWater()) return;
        if(isServer)
        {
            Attack(damagable.hp);            
        }
        else
        {
            CMDAttack(damagable.hp);
        }

        // kill events
        foreach (var item in funcs)
        {

            if(item == Funcs.MoveKill)
            {
                if(TryGetComponent(out UnitMovement movement0))
                {
                    StartCoroutine (movement0.MoveKill(damagable.Hex,damagable.hp.Hp<=0));
                }
            }
            if(item == Funcs.TakeHostage)
            {
                if(TryGetComponent(out UnitMovement movement1))
                {
                    movement1.TakeHostage(damagable.Hex,damagable.hp.Hp<=0);
                }
            }

            // switch (item)
            // {
                
            //     case Funcs.MoveKill:
            //     break;
            //     case Funcs.TakeHostage:
            //     break;
            // }
        }
        damagable.hp.Death(damagable,attackable,killEvent);
        
        Debug.Log("melee " + range);
        // AttackEvent?.Invoke();
        
    }
    [Command]
    protected void CMDAttack(HP hp)
    {
        Attack(hp);
    }
    [ClientRpc]
    public void Attack(HP hp)
    {
        if(hp != null)
        {
            hp.Hp -= _damagePower;

        }

        // InflictDamage(hp);
    }
}

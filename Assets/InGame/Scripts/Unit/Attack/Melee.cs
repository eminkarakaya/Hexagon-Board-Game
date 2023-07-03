using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Melee : Attack
{
    
    public override void AttackUnit(IDamagable damagable)
    {
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) return;
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
        damagable.hp.Death(damagable);
        
    }

    
    [Command]
    private void CMDAttack(HP hp)
    {
        Attack(hp);
    }
    [ClientRpc]
    public void Attack(HP hp)
    {
        if(hp != null)
        {
            hp.Hp -= _damagePower;
        Debug.Log("melee attack to : "  + hp);

        }

        // InflictDamage(hp);
    }
}

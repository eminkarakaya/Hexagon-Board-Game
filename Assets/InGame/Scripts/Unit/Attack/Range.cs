using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Range : Attack
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
        if(damagable.hp.TryGetComponent(out BuildingHP buildingHP)) return;
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
        if(hp != null && hp.Hp >= 0)
        {
            hp.Hp -= _damagePower;
        }

        // InflictDamage(hp);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Range : Attack
{

    public override void AttackUnit(IDamagable damagable,IAttackable attackable)
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
        damagable.hp.Death(damagable,attackable);
        // AttackEvent?.Invoke();
    }

    
    [Command]
    private void CMDAttack(HP hp)
    {
        Attack(hp);
        Debug.Log(hp.Hp +" hp.hpcmd");
    }
    [ClientRpc]
    public void Attack(HP hp)
    {
        if(hp != null && hp.Hp >= 0)
        {
            Debug.Log(hp.Hp +" hp.hpcmd");
            hp.Hp -= _damagePower;
        }

        // InflictDamage(hp);
    }
}


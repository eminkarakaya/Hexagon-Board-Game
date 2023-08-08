using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Range : Attack
{
                                    // ddamage alan      // damage atan
    public override IEnumerator AttackUnit(IDamagable damagable,IAttackable attackable,float movementDuration)
    {
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) yield break;
        if(isServer)
        {
            Attack(damagable.hp);            
        }
        else
        {
            CMDAttack(damagable.hp);
        }
        if(damagable.hp.TryGetComponent(out BuildingHP buildingHP)) yield break;
        damagable.hp.Death(damagable,attackable);
        Debug.Log("ranged + " + range);
        // AttackEvent?.Invoke();
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


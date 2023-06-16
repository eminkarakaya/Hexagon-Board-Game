using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Melee : Attack
{
    
    public override void AttackUnit(HP hp)
    {
        if(GetComponent<Unit>().GetCurrentMovementPoints()==0) return;
        if(isServer)
        {
            Attack(hp);
        }
        else
        {
            CMDAttack(hp);
        }
    }
    [Command]
    private void CMDAttack(HP hp)
    {
        Attack(hp);
    }
    [ClientRpc]
    public void Attack(HP hp)
    {

            hp.Hp -= _damagePower;
        Debug.Log("melee attack to : "  + hp);
        // InflictDamage(hp);
    }
}

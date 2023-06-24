using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : Attack
{

    public override void AttackUnit(IDamagable damagable)
    {
        if(GetComponent<Movement>().GetCurrentMovementPoints()==0) return;
        Debug.Log("range attack to : "  +damagable.hp);
    }
}

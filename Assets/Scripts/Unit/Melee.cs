using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Attack
{
    public override void AttackUnit(Unit unit)
    {
        if(GetComponent<Unit>().GetCurrentMovementPoints()==0) return;
        Debug.Log("melee attack to : "  + unit);
    }
}

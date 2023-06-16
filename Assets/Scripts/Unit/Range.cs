using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : Attack
{

    public override void AttackUnit(HP hp)
    {
        if(GetComponent<Unit>().GetCurrentMovementPoints()==0) return;
        Debug.Log("range attack to : "  + hp);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public int range = 1;
    public abstract void AttackUnit(Unit unit);
   
}

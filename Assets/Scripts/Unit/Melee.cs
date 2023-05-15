using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public void Attack(Unit unit)
    {
        Debug.Log("attak to : " + unit);
    }
}

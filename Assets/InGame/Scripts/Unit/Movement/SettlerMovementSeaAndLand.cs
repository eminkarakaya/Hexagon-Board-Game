using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlerMovementSeaAndLand : SettlerMovement
{
    protected override MovementSystem InitMovementSystem()
    {
        Debug.Log("calculate SEA AND LAND");
        return new SettlerMovementSystemSeaAndLand(Moveable);
    }
    
}

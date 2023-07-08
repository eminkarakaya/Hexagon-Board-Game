using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ShipMovement : UnitMovement

{
    protected override MovementSystem InitMovementSystem()
    {
        Debug.Log("calculate SEA AND LAND");
        return new ShipMovementSystem(Moveable);
    }
    
    [ClientRpc] protected override void RPCSetHex(Hex hex,Hex prevHex) 
    {
        if(TryGetComponent(out Ship ship))
        {
            prevHex.Ship = null;
            this.Moveable.Hex = hex;
            hex.Ship = ship;

        }
        
        else if(TryGetComponent(out Unit unit))
        {
            prevHex.Unit = null;
            this.Moveable.Hex = hex;
            hex.Unit = unit;
        }
        else if(TryGetComponent(out Settler settler))
        {
            prevHex.Settler = null;
            this.Moveable.Hex = hex;
            hex.Settler = settler;            
        }
        

    }
}

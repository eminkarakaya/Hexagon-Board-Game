using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
public interface IMovable
{
    public Movement Movement { get; set; }
    public MovementSystem Result { get; set; }
    
}
public class UnitMovableResult : UnitMovementSystem,IMovable
{
    public UnitMovableResult(Movement movement) : base(movement)
    {
        
    }
    public Movement Movement { get; set; }
    public MovementSystem Result { get ; set ; }
}

public class SettlerMovableResult :SettlerMovementSystem, IMovable
{
    public SettlerMovableResult(Movement movement) : base(movement)
    {        
        CalculateRange(movement,GameObject.FindObjectOfType<HexGrid>());
    }
    public Movement Movement { get; set; }
    public MovementSystem Result { get;  set; }
}

public interface ISelectable
{
    public Vector3Int Position { get; set; }
    public Side Side { get; set; }
    public Canvas Canvas { get; set; }
    public void OpenCanvas();
    public void CloseCanvas();
    public void LeftClick();
    public void RightClick(Hex selectedHex);
    public void RightClick2(Hex selectedHex);
    public void Select();
    public void Deselect();
    // public System.Action SelectEvent { get; set; }
}
public interface IAttackable
{
    public Vector3Int Position { get; set; }
    public Attack Attack { get; set; }
    public AttackSystem AttackSystem { get; set; }

}
public class MeeleAttack : AttackSystem,IAttackable
{
    public Vector3Int Position { get; set; }
    public Attack Attack { get; set; }
    public AttackSystem AttackSystem { get; set; }
}
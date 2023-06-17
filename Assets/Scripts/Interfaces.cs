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
    public Movement Movement { get; set; }
    public MovementSystem Result { get ; set ; }
}

public class SettlerMovableResult :SettlerMovementSystem, IMovable
{
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
    public void Select();
    public void Deselect();
    // public System.Action SelectEvent { get; set; }
}
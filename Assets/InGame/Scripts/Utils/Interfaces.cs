using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
public interface IDamagable
{
    public HP hp { get; set; }
    public Hex Hex { get; set; }
    
}
public interface IVisionable
{
    public List<GameObject> Visions{get;}
    public Vision Vision { get; set; }
    public Hex Hex { get; set; }
}
public interface IMovable
{
    public Movement Movement { get; set; }
    public MovementSystem Result { get; set; }
    public Outline Outline { get; set; } 
    public Hex Hex { get; set; }
    
}

public interface ISelectable
{
    public Side Side { get; set; }
    public Canvas Canvas { get; set; }
    public void OpenCanvas();
    public void CloseCanvas();
    public void LeftClick();
    public void RightClick(Hex selectedHex);
    public void RightClick2(Hex selectedHex);
    public void Deselect();
    public IMovable Movable { get; set; }
    public Hex Hex { get; set; }
    
    // public System.Action SelectEvent { get; set; }
}

public interface IAttackable
{
    public Hex Hex { get; set; }
    public Attack Attack { get; set; }
    public AttackSystem AttackSystem { get; set; }
    public CivManager CivManager { get; set; }

}
public class MeeleAttack : AttackSystem,IAttackable
{
    public Vector3Int Position { get; set; }
    public Attack Attack { get; set; }
    public AttackSystem AttackSystem { get; set; }
    public Hex Hex { get; set; }
    public CivManager CivManager { get; set; }
}
public interface ISideable
{
    public Hex Hex { get; set; }
    public CivManager CivManager { get; set; }
    public Side Side { get; set; }
    public Outline Outline { get; set; }
    public void SetSide(Side side , Outline outline);
}
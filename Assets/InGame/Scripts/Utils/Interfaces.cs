using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public interface IDamagable
{
    public List<PropertiesStruct> attackProperties{get;set;}
    public CivManager CivManager { get; set; }
    public HP hp { get; set; }
    public Hex Hex { get; set; }
    public bool IsBuisy { get; set; }
    
}
public interface ITaskable
{
    public Sprite OrderSprite { get; set; }
    public Transform Transform{ get; }
    public void LeftClick();
    public void TaskComplate();
    public void TaskReset();
}
public interface IVisionable
{
    public List<GameObject> Visions{get;}
    public Vision Vision { get; set; }
    public Hex Hex { get; set; }
}
public interface IMovable
{
    public bool IsBuisy { get; set; }
    public CivManager CivManager { get; set; }
    public void ToggleButtons(bool state);
    public Movement Movement { get; set; }
    public MovementSystem Result { get; set; }
    public OutlineObj Outline { get; set; } 
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
    public Hex Hex { get; set; }
    
}

public interface IAttackable
{
    public List<PropertiesStruct> attackProperties{get;set;}
    public Hex Hex { get; set; }
    public Attack Attack { get; set; }
    public AttackSystem AttackSystem { get; set; }
    public CivManager CivManager { get; set; }

}
public interface ISideable
{
    public Hex Hex { get; set; }
    public CivManager CivManager { get; set; }
    public Side Side { get; set; }
    public OutlineObj Outline { get; set; }
    public void SetSide(Side side , OutlineObj outline);
}

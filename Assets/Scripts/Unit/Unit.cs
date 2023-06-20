using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
[SelectionBase]
public class Unit : NetworkBehaviour , ISelectable, IMovable , IAttackable 
{
    public Attack Attack { get; set; }
    public AttackSystem AttackSystem { get; set; }
    public Movement Movement { get; set; }
    public HP hp;
    [SyncVar] [SerializeField] private Hex hex;
    public Hex Hex {get => hex; set {hex = value;}}
    [SerializeField] private Side side;
    public void SetSide(Side side,Outline outline)
    {
        this.side = side;
        if(outline == null) return;
        if(side == Side.Me)
        {
            outline.OutlineColor = Color.white;
        }
        else if(side == Side.Enemy)
        {
            outline.OutlineColor = Color.red;
        }
    }
    
    [SerializeField] private List<GameObject> sight;
    public List<GameObject> Sight{get => sight;}
    public MovementSystem Result { get ; set ; }
    public Vector3Int Position { get ; set ; }
    [SerializeField] Canvas canvas;
    public Canvas Canvas { get => canvas; set {canvas = value;} }
    public int Range { get; set; }
    public Outline outline { get; set; }
    public Side Side { get => side; set {side = value;} }

    private void Start() {
        hp = GetComponent<HP>();
        Movement = GetComponent<Movement>();
        Attack = GetComponent<Attack>();
        AttackSystem = new MeeleAttack();
        Result = new UnitMovableResult(this);
        
    }
    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);
    }
    public void CloseCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }
   
   
    public override void OnStartAuthority()
    {
        outline = GetComponent<Outline>();
    }
    public void RightClick(Hex selectedHex)
    {
        HexGrid hexGrid =FindObjectOfType<HexGrid>();
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid);
        Result.CalculateRange(this,hexGrid);
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid);
    } 
    public void RightClick2(Hex selectedHex)
    {
        Result.MoveUnit(Movement,FindObjectOfType<HexGrid>(),selectedHex);
    } 
    
    public void LeftClick()
    {
        outline.enabled = true;
        // Result = new UnitMovableResult(Movement);
        Result.ShowRange(this,Movement);
        AttackSystem.GetRange(this);
    }
    public void Deselect()
    {
        outline.enabled = false;
        Result.HideRange(this,Movement);
        AttackSystem.HideRange();
        
    }

    
}
public enum Side
{
    Ally,
    Me,
    Enemy
}
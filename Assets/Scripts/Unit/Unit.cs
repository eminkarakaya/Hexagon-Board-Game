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
    Outline outline;
    public HP hp;
    [SyncVar] [SerializeField] private Hex hex;
    public Hex Hex {get => hex; set {hex = value;}}
    [SerializeField] private GameObject canvas;
    [SerializeField] private Side side;
    public Side Side {get => side;}
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
    Side ISelectable.Side { get ; set ; }
    public Canvas Canvas { get ; set ; }
    public int Range { get; set; }

    private void Start() {
        hp = GetComponent<HP>();
        Movement = GetComponent<Movement>();
        Attack = GetComponent<Attack>();
        
    }
    public void OpenCanvas()
    {
        canvas.SetActive(true);
    }
    public void CloseCanvas()
    {
        canvas.SetActive(false);

    }
   
   
    public override void OnStartAuthority()
    {
        outline = GetComponent<Outline>();
    }
    public void RightClick(Hex selectedHex)
    {
        Result.ShowPath(selectedHex.HexCoordinates,FindObjectOfType<HexGrid>());
    } 
    public void RightClick2(Hex selectedHex)
    {
        Result.MoveUnit(Movement,FindObjectOfType<HexGrid>(),selectedHex);
    } 
    
    public void LeftClick()
    {
        Select();
    }

    public void Select()
    {
        outline.enabled = true;
        Result = new UnitMovableResult(Movement);
        Result.ShowRange(this,Movement);
         AttackSystem = new MeeleAttack();
        AttackSystem.GetRange(this);
    }   

    public void Deselect()
    {
        outline.enabled = false;
        Result.HideRange(Movement);
        AttackSystem.HideRange();
        
    }

    
}
public enum Side
{
    Ally,
    Me,
    Enemy
}
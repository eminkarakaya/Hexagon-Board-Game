using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
[SelectionBase]
public class Unit : NetworkBehaviour , ISelectable, IMovable
{
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

    public void Select()
    {
        outline.enabled = true;
        Result = new UnitMovableResult();
        HexGrid hexGrid = FindObjectOfType<HexGrid>();
        Result.ShowRange(Movement);
    }   

    public void Deselect()
    {
        outline.enabled = false;
    }

    
}
public enum Side
{
    Ally,
    Me,
    Enemy
}
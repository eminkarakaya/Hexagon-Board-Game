using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
[SelectionBase]
public class Unit : NetworkBehaviour , ISelectable, IMovable , IAttackable  , ISightable,IDamagable, ISideable
{
        [SyncVar] [SerializeField] private  CivManager civManager;
    public CivManager CivManager {get => civManager;set {civManager = value;}}

    public Attack Attack { get; set; }
    public AttackSystem AttackSystem { get; set; }
    public Movement Movement { get; set; }
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
    
   
    public MovementSystem Result { get ; set ; }
    public Vector3Int Position { get ; set ; }
    [SerializeField] Canvas canvas;
    public Canvas Canvas { get => canvas; set {canvas = value;} }
    public int Range { get; set; }
    public Outline Outline { get; set; }
    public Side Side { get => side; set {side = value;} }
    [SerializeField] private List<GameObject> sights;
    public List<GameObject> Sights=>sights;

    public Sight Sight { get; set; }
    public HP hp { get; set; }
    [SerializeField] private IMovable movable;
    public IMovable Movable { get => movable; set{movable = value;} }

    private void Start() {
        hp = GetComponent<HP>();
        Movement = GetComponent<Movement>();
        Attack = GetComponent<Attack>();
        AttackSystem = new MeeleAttack();
        Result = new UnitMovableResult(this);
        Outline = GetComponent<Outline>();
        Movable = GetComponent<IMovable>();
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
        Outline.enabled = true;
        Result = new UnitMovableResult(this);
        Result.ShowRange(this,Movement);
        AttackSystem.GetRange(this);
    }
    public void Deselect()
    {
        Outline.enabled = false;
        Result.HideRange(this,Movement);
        AttackSystem.HideRange();
        
    }



    [Command] private void CMDSetSide(NetworkIdentity identity,GameObject sideable)
    {
        RPGSetSide(identity,CivManager,sideable);
    }
    [ClientRpc] private void RPGSetSide(NetworkIdentity identity,CivManager civManager,GameObject sideable)
    {
        ISideable sideable1 = sideable.GetComponent<ISideable>();
        sideable1.CivManager = civManager;
        if(identity.isOwned)
        {
            sideable1.SetSide(Side.Me,sideable1.Outline);
        }
        else
        {
            sideable1.SetSide(Side.Enemy,sideable1.Outline);
        }
    }
    public void Capture(NetworkIdentity identity,GameObject _gameObject)
    {
        CivManager.Capture(identity);
        
        TeamColor [] teamColors = _gameObject.transform.GetComponentsInChildren<TeamColor>();
        foreach (var item in teamColors)
        {
            item.SetColor(CivManager.data);
        }
        StartCoroutine(wait(identity,_gameObject));
    }
    public IEnumerator wait(NetworkIdentity identity,GameObject sideable)
    {
        while(GetComponent<NetworkIdentity>().isOwned == false)
        {
            Debug.Log("kekw1");
            yield return null;
            
        }
        CMDSetSide(identity,sideable);
    }
    public void StartCoroutine1(NetworkIdentity identity,GameObject sideable)
    {
        StartCoroutine(wait(identity,sideable));
    }
}

public enum Side
{
    Ally,
    Me,
    Enemy
}
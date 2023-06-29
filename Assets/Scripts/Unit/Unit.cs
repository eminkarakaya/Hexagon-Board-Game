using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
[SelectionBase]
public class Unit : NetworkBehaviour , ISelectable, IMovable , IAttackable  , ISightable,IDamagable
{
    public CivManager civManager;
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
    public Outline outline { get; set; }
    public Side Side { get => side; set {side = value;} }
    [SerializeField] private List<GameObject> sights;
    public List<GameObject> Sights=>sights;

    public Sight Sight { get; set; }
    public HP hp { get; set; }

    private void Start() {
        hp = GetComponent<HP>();
        Movement = GetComponent<Movement>();
        Attack = GetComponent<Attack>();
        AttackSystem = new MeeleAttack();
        Result = new UnitMovableResult(this);
        if(civManager == null)
            civManager = PlayerManager.FindPlayerManager();
            
        civManager.SetTeamColor(this.gameObject);
        outline = GetComponent<Outline>();
        
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
        // Result.ShowPath(selectedHex.HexCoordinates,hexGrid);
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



    [Command] private void CMDSetSide(NetworkIdentity identity, Hex hex)
    {
        RPGSetSide(identity,hex,civManager);
    }
    [ClientRpc] private void RPGSetSide(NetworkIdentity identity,Hex hex,CivManager civManager)
    {
        hex.Settler.civManager = civManager;
        if(hex.Settler.isOwned)
        {
            hex.Settler.SetSide(Side.Me,hex.Settler.outline);
        }
        else
        {
            hex.Settler.SetSide(Side.Enemy,hex.Settler.outline);
        }
    }
    public void Capture(NetworkIdentity identity,Hex hex)
    {
        civManager.Capture(identity);
        
        TeamColor [] teamColors = hex.Settler.transform.GetComponentsInChildren<TeamColor>();
        foreach (var item in teamColors)
        {
            item.SetColor(civManager.data);
        }
        StartCoroutine(wait(hex.Settler,identity,hex));
    }
    IEnumerator wait(Settler settler,NetworkIdentity identity,Hex hex)
    {
        while(settler.isOwned == false)
        {
            Debug.Log("kekw");
            yield return null;
            
        }
        CMDSetSide(identity,hex);
    }
}

public enum Side
{
    Ally,
    Me,
    Enemy
}
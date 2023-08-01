using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
[SelectionBase]
public class Unit : NetworkBehaviour , ISelectable, IMovable , IAttackable  , IVisionable,IDamagable, ISideable,ITaskable
{
    #region PROPERTiES

    public Button pillageButton;
    [SyncVar] [SerializeField] private  CivManager civManager;
    public CivManager CivManager {get => civManager;set {civManager = value;}}
    public Attack Attack { get; set; }
    public AttackSystem AttackSystem { get; set; }
    public Movement Movement { get; set; }
    [SyncVar] [SerializeField] private Hex hex;
    public Hex Hex {get => hex; set {hex = value;}}
    [SerializeField] private Side side;
   
    
   
    public MovementSystem Result { get ; set ; }
    public Vector3Int Position { get ; set ; }
    [SerializeField] Canvas canvas;
    public Canvas Canvas { get => canvas; set {canvas = value;} }
    public int Range { get; set; }
    public Outline Outline { get; set; }
    public Side Side { get => side; set {side = value;} }
    [SerializeField] private List<GameObject> visions;
    public List<GameObject> Visions=>visions;

    public Vision Vision { get; set; }
    public HP hp { get; set; }
    [SerializeField] private IMovable movable;
    public IMovable Movable { get => movable; set{movable = value;} }

    public Transform Transform { get => transform;}
    [SerializeField] private Sprite orderImage;
    public Sprite OrderSprite { get =>orderImage; set {orderImage = value;} }
    #endregion
    #region Mirror and Unity Callback
    private void Start() {
        hp = GetComponent<HP>();
        Movement = GetComponent<Movement>();
        Attack = GetComponent<Attack>();
        AttackSystem = new AttackSystem();
        if(TryGetComponent(out ShipMovement shipMovement))
        {
            Result = new ShipMovementSystem(this);
        }
        else
            Result = new UnitMovementSystem(this);
        Outline = GetComponent<Outline>();
        Movable = GetComponent<IMovable>();
        pillageButton.onClick.AddListener(()=> PillageButton(Hex.gameObject));
    }
    public override void OnStopAuthority()
    {
        // CloseCanvas();
        // civManager.CMDHideAllUnits();
        // Movement.HideRangeStopAuthority();
        UnitManager.Instance.ClearOldSelection();
    }
    #endregion

    #region  SELECTABLE METHODS
    
    public int attackRange,moveRange;
    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);
    }
    public void CloseCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }
   
    
    protected void AttackUnit(Hex hex)
    {
        // Movement.StartCoroutineRotationUnit(Movement,hex.transform.position,hex);
        if(TryGetComponent(out Attack attack))
        {
            if(hex.Building != null && hex.Building.Side == Side.Enemy)
            {
                attack.AttackUnit(hex.Building,GetComponent<Unit>());
            }
            else if(hex.Unit != null && hex.Unit.Side == Side.Enemy)
            {
                attack.AttackUnit(hex.Unit,GetComponent<Unit>());
            }
            else if(hex.Ship != null && hex.Ship.Side == Side.Enemy)
            {

                attack.AttackUnit(hex.Ship,GetComponent<Ship>());
            }

        }
    }
    public bool CheckAttackOrMove(Hex selectedHex)
    {
        if(Movement.CurrentMovementPoints == 0)
            return false;
        if(moveRange == 0 && (selectedHex.IsEnemy()  || selectedHex.IsEnemyBuilding() || selectedHex.IsEnemyShip()))
        {
            Movement.StartCoroutineRotationUnit(Movement,selectedHex.transform.position,selectedHex);
            return true;
        }
        return false;
    }
    public void RightClick(Hex selectedHex)
    {
        HexGrid hexGrid =FindObjectOfType<HexGrid>();
        moveRange = Result.ShowPath(selectedHex.HexCoordinates,hexGrid,Attack.range).Count;
        
        if(CheckAttackOrMove(selectedHex))
        {
            AttackUnit(selectedHex);
        }   
        else
        {
            // Result.ShowPath(selectedHex.HexCoordinates,hexGrid,Attack.range);
            Result.CalculateRange(this,hexGrid);
            Result.MoveUnit(Movement,FindObjectOfType<HexGrid>(),selectedHex);
        }
        
    } 
    public void RightClick2(Hex selectedHex)
    {
        
    } 
    
    public void LeftClick()
    {
        Outline.enabled = true;
        if(TryGetComponent(out ShipMovement shipMovement))
        {
            Result = new ShipMovementSystem(this);
        }
        else
            Result = new UnitMovementSystem(this);
        Result.ShowRange(this,Movement);
        // AttackSystem.GetRange(this);
        attackRange = AttackSystem.ShowRange(this);
        // FindObjectOfType<HexGrid>().DrawBorders(Hex);
    }
    public void Deselect()
    {
        // Outline.enabled = false;
        Result.HideRange(this,Movement);
        AttackSystem.HideRange(this);
        
    }

    #endregion

    #region  SETSIDE

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
        else if(side == Side.Ally)
        {
            outline.OutlineColor = Color.blue;
        }
        else if(side == Side.None)  
        {
            outline.OutlineColor = Color.black;
        }
    }
    [Command] private void CMDSetSide(NetworkIdentity identity,GameObject sideable)
    {
        RPGSetSide(identity,CivManager,sideable);
    }
    [ClientRpc] private void RPGSetSide(NetworkIdentity identity,CivManager civManager,GameObject sideable)
    {
        ISideable sideable1 = sideable.GetComponent<ISideable>();
        sideable1.CivManager = civManager;

        if(civManager.isOwned)
        {
            sideable1.SetSide(Side.Me,sideable1.Outline);
        }
        else if(civManager.team == this.CivManager.team)
        {
            sideable1.SetSide(Side.Ally,sideable1.Outline);
        }
        else
        {
            sideable1.SetSide(Side.Enemy,sideable1.Outline);
        }
    }
    // public void Capture(NetworkIdentity identity,GameObject _gameObject)
    // {
    //     CivManager.Capture(identity);
        
    //     TeamColor [] teamColors = _gameObject.transform.GetComponentsInChildren<TeamColor>();
    //     foreach (var item in teamColors)
    //     {
    //         item.SetColor(CivManager.civData);
    //     }
    //     StartCoroutine(wait(identity,_gameObject));
    // }
    // public IEnumerator wait(NetworkIdentity identity,GameObject sideable)
    // {
    //     while(GetComponent<NetworkIdentity>().isOwned == false)
    //     {
    //         yield return null;
            
    //     }

    //     civManager.CMDHideAllUnits();
    //     CMDSetSide(identity,sideable);
    // }
    // public void StartCoroutine1(NetworkIdentity identity,GameObject sideable)
    // {
    //     StartCoroutine(wait(identity,sideable));
    // }
    protected virtual void ToggleButtonsVirtual(bool state)
    {

    }

    public void ToggleButtons(bool state)
    {
        ToggleButtonsVirtual(state);
    }

    public void TaskComplate()
    {
        CivManager.RemoveOrderList(this);
    }

    public void TaskReset()
    {
        civManager.AddOrderList(this);
        Movement.ResetMovementPoint();
    }
    #endregion

    public void TogglePillageButtonInteractableOpen()
    {
        pillageButton.interactable = true;
    }
    public void TogglePillageButtonInteractableClose()
    {
        pillageButton.interactable = false;
    }
    public void PillageButton(GameObject target)
    {
        if(target.TryGetComponent(out Hex hex))
        {   
            hex.resource.ChangeOwned(this);
        }
    }
}

public enum Side
{
    Ally,
    Me,
    Enemy,
    None
}
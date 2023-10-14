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

    [Header("UI")]
    [SerializeField] private Image unitImage;
    
    
    [Header("UI Data")]
    
    [SerializeField] private Sprite unitSprite;


    int attackRange;
    int? moveRange;
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
    [SyncVar] [SerializeField] private bool isBuisy;
    public bool IsBuisy { get => isBuisy;set{isBuisy = value;}}

    [SerializeField] private List<PropertiesStruct> _attackProperties;
    public List<PropertiesStruct> attackProperties { get => _attackProperties; set{_attackProperties = value;} }
    #endregion
    #region Mirror and Unity Callback

    private void OnValidate() {
        SkinnedMeshRenderer [] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if(!Visions.Contains(renderers[i].gameObject))
                Visions.Add(renderers[i].gameObject);
        }
    }
    private void Start() {
        hp = GetComponent<HP>();
        Movement = GetComponent<Movement>();
        Attack = GetComponent<Attack>();
        if(TryGetComponent(out ShipMovement shipMovement))
        {
            Result = new ShipMovementSystem(this);
        }
        else
            Result = new UnitMovementSystem(this);
        Outline = GetComponent<Outline>();
        Movable = GetComponent<IMovable>();
        pillageButton.onClick.AddListener(()=> PillageButton(Hex.gameObject.transform));
        UpdateUI();
    }
    public override void OnStopAuthority()
    {
        UnitManager.Instance.ClearOldSelection();
    }
    #endregion

    private void UpdateUI()
    {
        unitImage.sprite = unitSprite;
        
    }
    

    #region  SELECTABLE METHODS
    
    
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
        if(TryGetComponent(out Attack attack))
        {
            if(hex.Building != null && hex.Building.Side == Side.Enemy)
            {
                StartCoroutine (attack.AttackUnit(hex.Building,GetComponent<Unit>(),.2f));
            }
            else if(hex.Ship != null && hex.Ship.Side == Side.Enemy)
            {

                StartCoroutine (attack.AttackUnit(hex.Ship,GetComponent<Ship>(),.2f));
            }
            else if(hex.Unit != null && hex.Unit.Side == Side.Enemy)
            {
                StartCoroutine(attack.AttackUnit(hex.Unit,GetComponent<Unit>(),.2f));
            }

        }
    }

    public void StartCaptureCoroutine(NetworkIdentity identity,GameObject sideable,CivManager civManager)
    {
        StartCoroutine(CaptureCoroutine(civManager));
    }
    public IEnumerator CaptureCoroutine(CivManager attackableCivManager)
    {
        this.CivManager = attackableCivManager;
        while(GetComponent<NetworkIdentity>().isOwned == false)
        {
            yield return null;

        }
        CMDSetSide(attackableCivManager);
        attackableCivManager.CMDSetTeamColor(this.gameObject);
        
        attackableCivManager.CMDShowAllUnits();

    }
    [Command] public void CMDSetSide(CivManager civManager)
    {
        RPGSetSide(civManager);
    }
    [ClientRpc] private void RPGSetSide(CivManager civManager)
    {
        this.CivManager = civManager;
        if(civManager.isOwned)
        {
            SetSide(Side.Me,Outline);
        }
        else
        {
            SetSide(Side.None,Outline);
        }
    }
    public bool CheckAttackOrMove(Hex selectedHex)
    {
        if(Movement.CurrentMovementPoints == 0)
            return false;

        AttackSystem = new AttackSystem(this);
        if(moveRange == 0 &&  AttackSystem.CheckEnemyInRange(selectedHex) && (selectedHex.IsEnemy()  || selectedHex.IsEnemyBuilding() || selectedHex.IsEnemyShip()))
        {
            Movement.StartCoroutineRotationUnit(Movement,selectedHex.transform.position,selectedHex);
            return true;
        }
        return false;
    }
    public void RightClick(Hex selectedHex)
    {
        if(!isOwned) return;
        HexGrid hexGrid =FindObjectOfType<HexGrid>();
        List<Vector3Int> path = Result.ShowPath(selectedHex.HexCoordinates,hexGrid,Attack.range);
        
        if(path == null)
        {
            return;
        }
        moveRange = path.Count;
        if(moveRange == null)
        {
            return;
        }
        if(CheckAttackOrMove(selectedHex))
        {
            AttackUnit(selectedHex);
        }   
        else
        {
            Result.CalculateRange(this,hexGrid);
            Result.MoveUnit(Movement,FindObjectOfType<HexGrid>(),selectedHex);
        }
        UnitManager.Instance.ClearOldSelection();
        
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
        AttackSystem = new AttackSystem(this);
        attackRange = AttackSystem.ShowRange(this);
    }
    public void Deselect()
    {
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
            sideable1.SetSide(Side.None,sideable1.Outline);
        }
    }
        
            

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
    public void PillageButton(Transform target)
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
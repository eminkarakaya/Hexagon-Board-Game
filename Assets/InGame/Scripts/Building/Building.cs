using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Building : NetworkBehaviour , ISelectable ,IVisionable,IDamagable,ISideable
{
    
    #region Properties


    [Header("UI")]
    [SerializeField] private Image buildIcon;

    [Header("UI DATA")]
    [SerializeField] private Sprite unitSprite;

    [Space(10)]
    [SyncVar] [SerializeField] protected  CivManager _civManager;
    public CivManager CivManager {get => _civManager;set {_civManager = value;}}

    
    [Header("Prefabs")]
    [SerializeField] private GameObject shipRange;

    [SerializeField] private GameObject warrior,settler,archer,worker;
    [SerializeField] protected GameObject ship;
    public Hex Hex { get => hex; set{hex = value;} }
    [SyncVar] [SerializeField] private Hex hex = null;
    [SerializeField] private Side side;
    public Side Side {get => side;set{side = value;}}

    public Canvas Canvas { get => _canvas; set{_canvas = value;} }
    [SerializeField] private List<GameObject> visions;
    public List<GameObject> Visions => visions;

    public Vision Vision { get; set; }
    public HP hp { get; set; }
    public Outline Outline { get; set; }
    [SyncVar] [SerializeField] private bool isBuisy;
    public bool IsBuisy { get => isBuisy;set{isBuisy = value;}}

    public List<PropertiesStruct> AttackProperties { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    [SerializeField] Canvas _canvas;
    #endregion

    #region Mirror and Unity Callback
    private void OnValidate() {
        shipRange = Resources.Load<GameObject>("Units/Ship Range");
        warrior = Resources.Load<GameObject>("Units/Brute");
        archer = Resources.Load<GameObject>("Units/Erika");
        worker = Resources.Load<GameObject>("Settlers/Worker");
        ship = Resources.Load<GameObject>("Units/Ship");
        settler = Resources.Load<GameObject>("Settlers/Settler");
    }
    private IEnumerator Start() {
        Outline = GetComponent<Outline>();
        hp = GetComponent<HP>();
        Vision = GetComponent<Vision>();
        while(Hex == null)
        {
            yield return null;
        }
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.CMDHideAllUnits();
        CivManager.CMDShowAllUnits();
        UpdateUI();
    }

    public override void OnStopAuthority()
    {
        CloseCanvas();
    }

    #endregion
    private void UpdateUI()
    {
        buildIcon.sprite = unitSprite;
    }
   
    #region  CREATE Unit
    
    private void CreateUnit(Unit unit)
    {
        unit.Hex = Hex;
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.AddOrderList(unit);
        RPCCreateWarrior(unit);
        FindPlayerManager(unit);
        _civManager.CMDSetTeamColor(unit.gameObject);
    }
    #region  CMDCreate Units
    [Command] private void CMDCreateWarrior()
    {
        if(Hex.Unit != null) return;
        Unit unit = Instantiate(warrior,transform.position,Quaternion.identity).GetComponent<Unit>();
        CreateUnit(unit);
    }
    [Command] private void CMDCreateArcher()
    {
        if(Hex.Unit != null) return;
        Unit unit = Instantiate(archer,transform.position,Quaternion.identity).GetComponent<Unit>();
        CreateUnit(unit);
    }
    


    #endregion
    [ClientRpc] protected void FindPlayerManager(Unit unit)
    {
        unit.CivManager = this._civManager;
        StartCoroutine(FindPlayerManagerIE(unit));
    }
    private IEnumerator FindPlayerManagerIE(Unit unit)
    {
        while(unit.CivManager == null)
        {
            yield return null;
        }
        unit.CivManager.CMDSetTeamColor(this.gameObject);

    }
    
   
    [ClientRpc]
    private void RPCCreateWarrior(Unit unit)
    {
        unit.CivManager = _civManager;
        unit.Hex = Hex;
        unit.Hex.Unit = unit;
        unit.SetSide(side,unit.GetComponent<Outline>());
        CivManager.CMDAddOwnedObject(unit.gameObject);
        CivManager.CMDHideAllUnits();
        CivManager.CMDShowAllUnits();
    }
    public void CreateWarriorOnClick()
    {
        ToopltipManager.Hide();
        CMDCreateWarrior();
    }
    public void CreateArcherOnClick()
    {
        ToopltipManager.Hide();
        CMDCreateArcher();
    }
   
    #endregion
   
    [ClientRpc] private void FindPlayerManager(Settler unit)
    {
       
        unit.CivManager = this._civManager;
        StartCoroutine(FindPlayerManagerIE(unit));
    }
     private IEnumerator FindPlayerManagerIE(Settler unit)
    {
        while(unit.CivManager == null)
        {
            yield return null;
        }
        unit.CivManager.CMDSetTeamColor(this.gameObject);
    
    }
    #region  CREATE SETTLER


    [ClientRpc]
    private void RPCCreateSettler(Settler unit)
    {
        unit.Hex = Hex;
        unit.Hex.Settler = unit;
        _civManager.CMDAddOwnedObject(unit.gameObject);
        unit.SetSide(side,unit.GetComponent<Outline>());

        CivManager.CMDHideAllUnits();
        CivManager.CMDShowAllUnits();
    }
    private void CreateSettler(Settler settler)
    {
        settler.Hex = Hex;
        NetworkServer.Spawn(settler.gameObject,connectionToClient);
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.AddOrderList(settler);
        settler.CivManager = _civManager;
        RPCCreateSettler(settler);
        _civManager.CMDSetTeamColor(settler.gameObject);
    }
    [Command]
    private void CMDCreateSettler()
    {
        if(Hex.Settler != null) return;
        Settler unit = Instantiate(settler,transform.position,Quaternion.identity).GetComponent<Settler>();
        CreateSettler(unit);
    }
     [Command]
    private void CMDCreateWorker()
    {
        if(Hex.Settler != null) return;
        Settler unit = Instantiate(worker,transform.position,Quaternion.identity).GetComponent<Settler>();
        CreateSettler(unit);
    }
    
    public void CreateSettlerOnClick()
    {
        ToopltipManager.Hide();
        CMDCreateSettler();
    }

    public void CreateWorkerOnClick()
    {
        ToopltipManager.Hide();
        CMDCreateWorker();
    }

    #endregion





    #region  SetSide
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
            SetSide(Side.Enemy,Outline);
        }
        
    }
    
    public IEnumerator wait(CivManager attackableCivManager)
    {
        this.CivManager = attackableCivManager;
        while(GetComponent<NetworkIdentity>().isOwned == false)
        {
            yield return null;
        }
        
        CMDSetSide(attackableCivManager);
        attackableCivManager.CMDShowAllUnits();
    }
    public void SetSide(Side side, Outline outline)
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
    #endregion

    #region CREATEShip
    [Command] public void CMDCreateShip()
    {
        if(!hex.isCoast && hex.IsBuilding()) return;
        if(Hex.Ship != null) return;
        Ship unit = Instantiate(ship,transform.position,Quaternion.identity).GetComponent<Ship>();
        CreateShip(unit);
    }
    [Command] public void CMDCreateShipRange()
    {
        if(!hex.isCoast && hex.IsBuilding()) return;
        if(Hex.Ship != null) return;
        Ship unit = Instantiate(shipRange,transform.position,Quaternion.identity).GetComponent<Ship>();
        CreateShip(unit);
    }
    private void CreateShip(Ship ship)
    {
        ship.Hex = Hex;
        NetworkServer.Spawn(ship.gameObject,connectionToClient);
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.AddOrderList(ship);
        CivManager.CMDAddOwnedObject(ship.gameObject);
        RPCCreateShip(ship);
        FindPlayerManager(ship);
        _civManager.CMDSetTeamColor(ship.gameObject);
    }
    [ClientRpc]
    private void RPCCreateShip(Ship ship)
    {
        ship.CivManager = _civManager;
        ship.Hex = Hex;
        ship.Hex.Ship = ship;
        _civManager.CMDAddOwnedObject(ship.gameObject);
        
        ship.SetSide(side,ship.GetComponent<Outline>());
       
        CivManager.CMDHideAllUnits();
        CivManager.CMDShowAllUnits();
    }
    public void CreateShipOnClick()
    {
        ToopltipManager.Hide();
        CMDCreateShip();
    }
    public void CreateShipOnClickRange()
    {
        ToopltipManager.Hide();
        CMDCreateShipRange();
    }

    #endregion

    
   
    #region  SELECTABLE METHODS

    public void LeftClick()
    {
        OpenCanvas();
    }

    public void RightClick(Hex selectedHex)
    {
        
    }

    public void RightClick2(Hex selectedHex)
    {
        
    }

    public void Deselect()
    {
        
    }
    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);
    }
    public void CloseCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }
    #endregion
    

}

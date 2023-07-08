using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Building : NetworkBehaviour , ISelectable ,IVisionable,IDamagable,ISideable
{
    
    #region Properties

    [SyncVar] [SerializeField] private  CivManager civManager;
    public CivManager CivManager {get => civManager;set {civManager = value;}}

    
    [Header("Prefabs")]

    [SerializeField] private GameObject mc1,settler,mc1Range,harborSettler,ship;
    public Hex Hex { get => hex; set{hex = value;} }
    [SyncVar] [SerializeField] private Hex hex = null;
    [SerializeField] private Side side;
    public Side Side {get => side;set{side = value;}}

    public Vector3Int Position { get; set; }
    public Canvas Canvas { get => _canvas; set{_canvas = value;} }
    [SerializeField] private List<GameObject> visions;
    public List<GameObject> Visions => visions;

    public Vision Vision { get; set; }
    public HP hp { get; set; }
    public Outline Outline { get; set; }
    public IMovable Movable { get; set; }

    [SerializeField] UnityEngine.Canvas _canvas;
    #endregion

    #region Mirror and Unity Callback
    public override void OnStartClient()
    {
        Outline = GetComponent<Outline>();
        hp = GetComponent<HP>();
        Vision = GetComponent<Vision>();

    }
    private IEnumerator Start() {
        while(Hex == null)
        {
            yield return null;
        }
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.CMDHideAllUnits();
        CivManager.CMDShowAllUnits();
    }

    public override void OnStopAuthority()
    {
        CloseCanvas();
    }

    #endregion
   
    #region  CREATE Unit
    private void CreateShip(Ship ship)
    {
        ship.Hex = Hex;
        // RPCSetHex(ship,Hex);
        NetworkServer.Spawn(ship.gameObject,connectionToClient);
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.ownedObjs.Add(ship.gameObject);
        RPCCreateShip(ship);
        FindPlayerManager(ship);
        civManager.SetTeamColor(ship.gameObject);
    }

    private void CreateUnit(Unit unit)
    {
        unit.Hex = Hex;
        // RPCSetHex(unit,Hex);
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.ownedObjs.Add(unit.gameObject);
        RPCCreateMC1(unit);
        FindPlayerManager(unit);
        civManager.SetTeamColor(unit.gameObject);
    }
    #region  CMDCreate Units
    [Command] private void CMDCreateMC1()
    {
        if(Hex.Unit != null) return;
        Unit unit = Instantiate(mc1,transform.position,Quaternion.identity).GetComponent<Unit>();
        CreateUnit(unit);
    }
    [Command] private void CMDCreateMC1Range()
    {
        if(Hex.Unit != null) return;
        Unit unit = Instantiate(mc1Range,transform.position,Quaternion.identity).GetComponent<Unit>();
        CreateUnit(unit);
    }
    [Command] private void CMDCreateShip()
    {
        if(Hex.Ship != null) return;
        Ship unit = Instantiate(ship,transform.position,Quaternion.identity).GetComponent<Ship>();
        CreateShip(unit);
    }


    #endregion
    [ClientRpc] private void FindPlayerManager(Unit unit)
    {
        unit.CivManager = this.civManager;
        StartCoroutine(FindPlayerManagerIE(unit));
    }
    private IEnumerator FindPlayerManagerIE(Unit unit)
    {
        while(unit.CivManager == null)
        {
            yield return null;
        }
        unit.CivManager.SetTeamColor(this.gameObject);

    }
    
    [ClientRpc]
    private void RPCCreateShip(Ship ship)
    {
        ship.CivManager = civManager;
        ship.Hex = Hex;
        ship.Hex.Ship = ship;
        if(ship.isOwned)
        {
            ship.SetSide(Side.Me,ship.GetComponent<Outline>());
        }
        else
        {
            ship.SetSide(Side.Enemy,ship.GetComponent<Outline>());
        }
        
       
        CivManager.CMDHideAllUnits();
        CivManager.CMDShowAllUnits();
    }
    [ClientRpc]
    private void RPCCreateMC1(Unit unit)
    {
        unit.CivManager = civManager;
        unit.Hex = Hex;
        unit.Hex.Unit = unit;
        if(unit.isOwned)
        {
            unit.SetSide(Side.Me,unit.GetComponent<Outline>());
        }
        else
        {
            unit.SetSide(Side.Enemy,unit.GetComponent<Outline>());
        }
        
       
        CivManager.CMDHideAllUnits();
        CivManager.CMDShowAllUnits();
    }
    public void CreateMC1OnClick()
    {
        CMDCreateMC1();
    }
    public void CreateMC1OnClickRange()
    {
        CMDCreateMC1Range();
    }
    public void CreateShipOnClickRange()
    {
        CMDCreateShip();
    }
    #endregion
   
    
     #region  CREATE SETTLER

    [ClientRpc] private void FindPlayerManager(Settler unit)
    {
       
        unit.CivManager = this.civManager;
        StartCoroutine(FindPlayerManagerIE(unit));
    }
     private IEnumerator FindPlayerManagerIE(Settler unit)
    {
        while(unit.CivManager == null)
        {
            yield return null;
        }
        unit.CivManager.SetTeamColor(this.gameObject);

    }
    [ClientRpc]
    private void RPCCreateSettler(Settler unit)
    {
        unit.Hex = Hex;
        unit.Hex.Settler = unit;
        if(unit.isOwned)
        {
            unit.SetSide(Side.Me,unit.GetComponent<Outline>());
        }
        else
        {
            unit.SetSide(Side.Enemy,unit.GetComponent<Outline>());
        }
        

        CivManager.CMDHideAllUnits();
        CivManager.CMDShowAllUnits();
    }
    private void CreateSettler(Settler settler)
    {
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        settler.CivManager = civManager;
        settler.Hex = Hex;
        NetworkServer.Spawn(settler.gameObject,connectionToClient);
        RPCCreateSettler(settler);
    }
    [Command]
    private void CMDCreateSettler()
    {
        if(Hex.Settler != null) return;
        Settler unit = Instantiate(settler,transform.position,Quaternion.identity).GetComponent<Settler>();
        CreateSettler(unit);
    }
     [Command]
    private void CMDCreateHarborSettler()
    {
        if(Hex.Settler != null) return;
        Settler unit = Instantiate(harborSettler,transform.position,Quaternion.identity).GetComponent<Settler>();
        CreateSettler(unit);
    }
    
    public void CreateSettlerOnClick()
    {
        CMDCreateSettler();
    }

    public void CreateHarborSettlerOnClick()
    {
        CMDCreateHarborSettler();
    }

    #endregion





    #region  SetSide
    [Command] public void CMDSetSide(NetworkIdentity identity, GameObject _gameObject,CivManager civManager)
    {
        RPGSetSide(identity,_gameObject,civManager);
    }
    [ClientRpc] private void RPGSetSide(NetworkIdentity identity,GameObject _gameObject,CivManager civManager)
    {
        ISideable sideable = _gameObject.GetComponent<ISideable>();
        sideable.CivManager = civManager;
        if(civManager.isOwned)
        {
            sideable.SetSide(Side.Me,sideable.Outline);
        }
        else
        {
            sideable.SetSide(Side.Enemy,sideable.Outline);
        }
    }
    public IEnumerator wait(NetworkIdentity identity,GameObject sideable,CivManager civManager)
    {
        while(sideable.GetComponent<NetworkIdentity>().isOwned == false)
        {
            yield return null;
            
        }
        CMDSetSide(identity,sideable,civManager);
        civManager.CMDShowAllUnits();
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
        else
            outline.OutlineColor = Color.blue;

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

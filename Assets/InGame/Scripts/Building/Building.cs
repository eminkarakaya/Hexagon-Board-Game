using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Building : NetworkBehaviour , ISelectable ,ISightable,IDamagable,ISideable
{
    
    #region Properties

    [SyncVar] [SerializeField] private  CivManager civManager;
    public CivManager CivManager {get => civManager;set {civManager = value;}}

    
    [Header("Prefabs")]

    [SerializeField] private GameObject mc1,settler,mc1Range;
    public Hex Hex { get => hex; set{hex = value;} }
    [SyncVar] [SerializeField] private Hex hex = null;
    [SerializeField] private Side side;
    public Side Side {get => side;set{side = value;}}

    public Vector3Int Position { get; set; }
    public Canvas Canvas { get => _canvas; set{_canvas = value;} }
    [SerializeField] private List<GameObject> sights;
    public List<GameObject> Sights => sights;

    public Sight Sight { get; set; }
    public HP hp { get; set; }
    public Outline Outline { get; set; }
    public IMovable Movable { get; set; }

    [SerializeField] UnityEngine.Canvas _canvas;
    #endregion

    #region Mirror and Unity Callback
   
    private void Start() {
        Outline = GetComponent<Outline>();
        hp = GetComponent<HP>();
        Sight = GetComponent<Sight>();
    }

    public override void OnStopAuthority()
    {
        CloseCanvas();
    }

    #endregion
   
    #region  CREATE Unit
    [Command]
    private void CMDCreateMC1Range()
    {
        if(Hex.Unit != null) return;
        Unit unit = Instantiate(mc1,transform.position,Quaternion.identity).GetComponent<Unit>();
        unit.Hex = Hex;
        // RPCSetHex(unit,Hex);
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.ownedObjs.Add(unit.gameObject);
        RPCCreateMC1(unit);
        FindPlayerManager(unit);
        // AddLiveUnits(unit);
    }
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
    private void RPCCreateMC1Range(Unit unit)
    {
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
    public void CreateMC1OnClickRange()
    {
        CMDCreateMC1();
    }
    #endregion
    #region  CREATE RANGEMC1
    [Command]
    private void CMDCreateMC1()
    {
        if(Hex.Unit != null) return;
        Unit unit = Instantiate(mc1Range,transform.position,Quaternion.identity).GetComponent<Unit>();
        unit.Hex = Hex;
        // RPCSetHex(unit,Hex);
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        CivManager.ownedObjs.Add(unit.gameObject);
        RPCCreateMC1(unit);
        FindPlayerManager(unit);
        // AddLiveUnits(unit);
    }
    private IEnumerator FindPlayerManagerIERange(Unit unit)
    {
        while(unit.CivManager == null)
        {
            yield return null;
        }
        unit.CivManager.SetTeamColor(this.gameObject);

    }
    
    [ClientRpc]
    private void RPCCreateMC1(Unit unit)
    {
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
        if(identity.isOwned)
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
            Debug.Log("kekw building");
            yield return null;
            
        }
        CMDSetSide(identity,sideable,civManager);
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
    [Command]
    private void CMDCreateSettler()
    {
        if(CivManager == null)
            CivManager = PlayerManager.FindPlayerManager();
        if(Hex.Settler != null) return;
        Settler unit = Instantiate(settler,transform.position,Quaternion.identity).GetComponent<Settler>();
        unit.Hex = Hex;
        // RPCSetHex(unit,Hex);
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateSettler(unit);
        FindPlayerManager(unit);
        // AddLiveUnits(unit.GetComponent<IMovable>());
    }
    
    public void CreateSettlerOnClick()
    {
        CMDCreateSettler();
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
        // Canvas.gameObject.SetActive(false);
    }
    #endregion
    

}

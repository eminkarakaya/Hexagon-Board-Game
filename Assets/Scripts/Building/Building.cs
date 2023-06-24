using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Building : NetworkBehaviour , ISelectable ,ISightable,IDamagable
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject mc1,settler;
    public Hex Hex { get => hex; set{hex = value;} }
    [SyncVar] private Hex hex;
    [SerializeField] private Side side;
    public Side Side {get => side;set{side = value;}}

    public Vector3Int Position { get; set; }
    public Canvas Canvas { get => _canvas; set{_canvas = value;} }
    [SerializeField] private List<GameObject> sights;
    public List<GameObject> Sights => sights;

    public Sight Sight { get; set; }
    public HP hp { get; set; }

    [SerializeField] UnityEngine.Canvas _canvas;

    private void Start() {
        hp = GetComponent<HP>();
        Sight = GetComponent<Sight>();
    }
    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);
    }
    public void CloseCanvas()
    {
        // Canvas.gameObject.SetActive(false);
    }
    [ClientRpc] private void RPCSetHex(Unit unit,Hex hex)
    {
        unit.Hex = hex;
        hex.Unit = unit;
    }
    // private void AddLiveUnits(IMovable unit)
    // {
    //     playerManager = FindObjectOfType<PlayerManager>();
    //     playerManager.liveUnits.Add(unit);
    //     HexGrid hexGrid = FindObjectOfType<HexGrid>();
    //     // hexGrid.CloseVisible();
    //     // unit.Hex = Hex;
    // }
    [Command]
    private void CMDCreateMC1()
    {
        if(Hex.Unit != null) return;
        Unit unit = Instantiate(mc1,transform.position,Quaternion.identity).GetComponent<Unit>();
        unit.Hex = Hex;
        // RPCSetHex(unit,Hex);
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateMC1(unit);
        // AddLiveUnits(unit);
    }
    [ClientRpc]
    private void RPCCreateMC1(Unit unit)
    {
        unit.Hex = Hex;
        unit.Hex.Unit = unit;
        if(unit.isOwned)
        {
            unit.GetComponent<ISelectable>().SetSide(Side.Me,unit.GetComponent<Outline>());
        }
        else
        {
            unit.GetComponent<ISelectable>().SetSide(Side.Enemy,unit.GetComponent<Outline>());
        }
        playerManager = FindObjectOfType<PlayerManager>();

        playerManager.CMDHideAllUnits();
        playerManager.CMDShowAllUnits();
    }
    public void CreateMC1OnClick()
    {
        CMDCreateMC1();
    }
    [ClientRpc]
    private void RPCCreateSettler(Settler unit)
    {
        unit.Hex = Hex;
        unit.Hex.Settler = unit;
        if(unit.isOwned)
        {
            unit.GetComponent<ISelectable>().SetSide(Side.Me,unit.GetComponent<Outline>());
        }
        else
        {
            unit.GetComponent<ISelectable>().SetSide(Side.Enemy,unit.GetComponent<Outline>());
        }
        playerManager = FindObjectOfType<PlayerManager>();

        playerManager.CMDHideAllUnits();
        playerManager.CMDShowAllUnits();
    }
    [Command]
    private void CMDCreateSettler()
    {
        if(Hex.Unit != null) return;
        Settler unit = Instantiate(settler,transform.position,Quaternion.identity).GetComponent<Settler>();
        unit.Hex = Hex;
        // RPCSetHex(unit,Hex);
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateSettler(unit);
        // AddLiveUnits(unit.GetComponent<IMovable>());
    }
    
    public void CreateSettlerOnClick()
    {
        CMDCreateSettler();
    }

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Building : NetworkBehaviour 
{
    public HP hp;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject mc1;
    [SyncVar] public Hex Hex;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Side side;
    public Side Side {get => side;}
    private void Start() {
        hp = GetComponent<HP>();
    }
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
   
    public void OpenCanvas()
    {
        canvas.SetActive(true);
    }
    public void CloseCanvas()
    {
        canvas.SetActive(false);

    
    }
    [ClientRpc] private void RPCSetHex(Unit unit,Hex hex)
    {
        unit.Hex = hex;
        hex.Unit = unit;
    }
    [TargetRpc] private void AddLiveUnits(Unit unit)
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.liveUnits.Add(unit);
        HexGrid hexGrid = FindObjectOfType<HexGrid>();
        // hexGrid.CloseVisible();
        unit.Hex = Hex;
        
    }
    [Command]
    private void CMDCreateMC1()
    {
        Unit unit = Instantiate(mc1,transform.position,Quaternion.identity).GetComponent<Unit>();
        unit.Hex = Hex;
        // RPCSetHex(unit,Hex);
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateMC1(unit);
        AddLiveUnits(unit);
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
        playerManager = FindObjectOfType<PlayerManager>();

        playerManager.CMDHideAllUnits();
        playerManager.CMDShowAllUnits();
    }
    public void CreateMC1OnClick()
    {
        CMDCreateMC1();
    }
}

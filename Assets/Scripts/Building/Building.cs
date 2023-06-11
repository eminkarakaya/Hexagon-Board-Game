using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Building : NetworkBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject mc1;
    public Hex Hex;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Side side;
    public Side Side {get => side;}
    public void SetSide(Side side)
    {
        
       this.side = side;
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
        Debug.Log(playerManager + " " + unit);
        playerManager.liveUnits.Add(unit);
    }
    [Command]
    private void CMDCreateMC1()
    {
        Unit unit = Instantiate(mc1,transform.position,Quaternion.identity).GetComponent<Unit>();
        unit.Hex = Hex;
        // RPCSetHex(unit,Hex);
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        AddLiveUnits(unit);
        RPCCreateMC1(unit);
    }
    [ClientRpc]
    private void RPCCreateMC1(Unit unit)
    {
        unit.Hex = Hex;
        unit.Hex.Unit = unit;
        if(unit.isOwned)
        {
            unit.SetSide(Side.Me);
        }
        else
        {
            unit.SetSide(Side.Enemy);

        }
    }
    public void CreateMC1OnClick()
    {
        CMDCreateMC1();
        if(!isOwned)
        {
            // NetworkServer.AddPlayerForConnection(PlayerManager.Instance.connectionToClient,this.gameObject);
        }
        if(!isLocalPlayer) return;
    }
}

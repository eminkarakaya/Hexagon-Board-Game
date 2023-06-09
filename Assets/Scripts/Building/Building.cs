using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Building : NetworkBehaviour
{
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
    public override void OnStartClient()
    {
        base.OnStartClient();
        // Debug.Log(PlayerManager.Instance.connectionToClient);
    }
    public void CreateMC1OnClick()
    {
        if(!isOwned)
        {
            // NetworkServer.AddPlayerForConnection(PlayerManager.Instance.connectionToClient,this.gameObject);
        }
        if(!isLocalPlayer) return;
        
        // Unit unit = PhotonNetwork.Instantiate("MC1",transform.position,Quaternion.identity,0,null).GetComponent<Unit>();
        // unit.SetSide(side);
        // unit.Hex = Hex;
        // unit.GetComponent<PhotonView>().Owner.NickName = GameManager.Instance.NickName;
        // // unit.gameObject.GetComponent<PhotonView>().RPC("AssignSideForOtherPlayers",RpcTarget.All,null);
        // AssignSideForOtherPlayers();
        // qwe();
    }
    [Command]
    public void AssignClient(NetworkConnectionToClient conn)
    {
        // Debug.Log(pm.connectionToClient + " connectionToClient");
        NetworkServer.ReplacePlayerForConnection(PlayerManager.Instance.connectionToClient,this.gameObject);
    }

    public void qwe()
    {
        CMDCreateMC1();
    }
    
    
    [Server]
    public void CreateMC1Server()
    {
        Unit unit = Instantiate(mc1,transform.position,Quaternion.identity).GetComponent<Unit>();
        unit.Hex = Hex;
        NetworkServer.Spawn(unit.gameObject,connectionToServer);
    }
    [Command]
    public void CreateMC1Client()
    {
        Unit unit = Instantiate(mc1,transform.position,Quaternion.identity).GetComponent<Unit>();
        unit.Hex = Hex;
        NetworkServer.Spawn(unit.gameObject,connectionToServer);
    }
    
    private void CMDCreateMC1()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        if(networkIdentity.isServer)
        {
            CreateMC1Server();
        }

        else
        {
            CreateMC1Client();
        }
        PlayerManager playerManager = networkIdentity.GetComponent<PlayerManager>();
        
    }
}

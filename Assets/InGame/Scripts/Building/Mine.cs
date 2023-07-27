using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Mine : NetworkBehaviour, ISelectable, IVisionable, ISideable
{
    [SerializeField] Outline outline;
    Resource resource;
    [SerializeField] UnityEngine.Canvas _canvas;
    [SerializeField] private  CivManager civManager;
    public CivManager CivManager {get => civManager;set {civManager = value;}}
    [SerializeField] private Side side;
    public Side Side {get => side;set{side = value;}}
    public Canvas Canvas { get => _canvas; set{_canvas = value;} }
    public Hex Hex { get => hex; set{hex = value;} }
    [SyncVar] [SerializeField] private Hex hex = null;

    [SerializeField] private List<GameObject> visions;
    public List<GameObject> Visions => visions;

    public Vision Vision { get; set; }
    public Outline Outline { get => outline; set{outline = value;} }


    public void InitializeMine() {
        resource = Hex.resource;
        if(isOwned)
        {
            CMDOpenActive(resource);
            Debug.Log("isowned");
        }
        Outline = GetComponent<Outline>();
    }
    [Command] private void CMDOpenActive(Resource resource)
    {
        RPCOpenActive(resource);
        resource.CMDAddGold(CivManager);
    }
    [ClientRpc] private void RPCOpenActive(Resource resource)
    {
        resource.RPCOpenActive(CivManager);
        
    }
    #region  selectable Methods
    public void CloseCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }

    public void Deselect()
    {
        
    }

    public void LeftClick()
    {
        OpenCanvas();
    }

    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);
    }

    public void RightClick(Hex selectedHex)
    {
        
    }

    public void RightClick2(Hex selectedHex)
    {
        
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
    public IEnumerator wait(CivManager attackableCivManager)
    {
        // action?.Invoke();
        this.CivManager.CMDRemoveOwnedObject(this.gameObject); //requestauthority = false
        this.CivManager = attackableCivManager;
        while(GetComponent<NetworkIdentity>().isOwned == false)
        {
            yield return null;
        }
        
        // ele gecırıldıkten sonra
        CMDSetSide(attackableCivManager);
        attackableCivManager.CMDShowAllUnits();
        attackableCivManager.CMDAddOwnedObject(this.gameObject); //requestauthority = false
    }
    [Command] public void CMDSetSide(CivManager civManager)
    {
        RPGSetSide(civManager);
        
    }
    [ClientRpc] private void RPGSetSide(CivManager civManager)
    {
        if(civManager.isOwned)
        {
            SetSide(Side.Me,Outline);
        }
        else
        {
            SetSide(Side.Enemy,Outline);
        }
    }
    
}

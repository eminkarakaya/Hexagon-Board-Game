using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using UnityEngine.UI;

public class Settler : NetworkBehaviour , IMovable , ISelectable ,IVisionable ,ISideable,ITaskable
{
    #region  Props

    [SerializeField] private Sprite orderSprite;
    public Sprite OrderSprite { get =>orderSprite; set {orderSprite = value;} }
    [SyncVar] [SerializeField] private  CivManager civManager;
    public CivManager CivManager {get => civManager;set {civManager = value;}}
    public Transform Transform { get => transform;}
    [SerializeField] protected GameObject buildingPrefab,harborPrefab;
    public Hex Hex { get => _hex; set{_hex = value;} }
    [SerializeField] private Hex _hex;
    public Movement Movement { get; set; }
    public MovementSystem Result { get; set; }
    public Vector3Int Position { get; set; }
    [SerializeField] Canvas canvas;
    public Canvas Canvas { get => canvas; set{canvas = value;} }
    public OutlineObj Outline { get; set; }
    public Side Side { get =>_side; set{_side = value;} }
    [SerializeField] private List<GameObject> visions;
    public List<GameObject> Visions=>visions;

    public Vision Vision { get; set; }
    public IMovable Movable { get; set; }

    [SerializeField] private Side _side;
    [SyncVar] [SerializeField] private bool isBuisy;
    public bool IsBuisy { get => isBuisy;set{isBuisy = value;}}

    public List<PropertiesStruct> attackProperties { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    #endregion

    #region Mirror and Unity callbacks


    private void OnValidate() {
        SkinnedMeshRenderer [] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if(!Visions.Contains(renderers[i].gameObject))
                Visions.Add(renderers[i].gameObject);
        }
    }
    public override void OnStopAuthority()
    {
        UnitManager.Instance.ClearOldSelection();
    }
    private void Start() {
        Outline = GetComponent<OutlineObj>();
        Movement = GetComponent<Movement>();
        if(TryGetComponent(out SettlerMovementSeaAndLand settlerMovementSeaAndLand))
        {
            Result = new SettlerMovementSystemSeaAndLand(this);
        }
        else
            Result = new SettlerMovementSystem(this);
    }
    #endregion
    
    #region  selectable methods
    public void CloseCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }
    public virtual void SelectSettler()
    {
    }
    public void DeselectSettler()
    {
        Result.HideRange(this,Movement);

    }
    public void Deselect()
    {
        DeselectSettler();
    }


    public void LeftClick()
    {
        Outline.enabled = true;
        Result.ShowRange(this,Movement);
        SelectSettler();
    }

    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);

    }

    public void RightClick(Hex selectedHex)
    {
        if(!isOwned) return;
        if(Movement.CurrentMovementPoints == 0) return;
        HexGrid hexGrid =FindObjectOfType<HexGrid>();
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid,1);
        if(TryGetComponent(out SettlerMovementSeaAndLand settlerMovementSeaAndLand))
        {
            Result = new SettlerMovementSystemSeaAndLand(this);
        }
        else
            Result = new SettlerMovementSystem(this);
        Result.CalculateRange(this,hexGrid);
        List<Vector3Int> path = Result.ShowPath(selectedHex.HexCoordinates,hexGrid,1);

        if(path.Count != 0)
            Result.MoveUnit(Movement,FindObjectOfType<HexGrid>(),hexGrid.GetTileAt (path[path.Count-1]));
        UnitManager.Instance.ClearOldSelection();
    }

    public void RightClick2(Hex selectedHex)
    {
    }
    #endregion
    
    #region  createBuilding

    public void CreateBuildingOnClick()
    {
        ToopltipManager.Hide();
        
        if(Hex.isCoast )
        {
            CMDCreateHarbor();
        }
        else
        {
            CMDCreateBuilding();
        }
        TaskComplate();
        civManager.CMDRemoveOwnedObject(this.gameObject);
        civManager.CMDRemoveOrderListDontDestroy(this.gameObject,this.gameObject);
        
    }
    [Command]
    public virtual void CMDCreateHarbor()
    {
        
        if(Hex.Building != null) return;
        Harbor harbor = Instantiate(harborPrefab).GetComponent<Harbor>();
        NetworkServer.Spawn(harbor.gameObject,connectionToClient);
        RPCCreateBuilding(harbor);
    }
    [Command]
    public virtual void CMDCreateBuilding()
    {
        
        if(Hex.Building != null) return;
        Building building = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(building.gameObject,connectionToClient);
        RPCCreateBuilding(building);
    }
    [ClientRpc]
    protected void RPCCreateBuilding(Building building)
    {
        Debug.Log(building.transform.position,building);
        building.transform.position = new Vector3 (Hex.transform.position.x , 1 , Hex.transform.position.z );
        building.transform.rotation = Quaternion.Euler(-90,0,0);
        building.Hex = Hex;
        building.Hex.Building = building;
        building.CivManager = civManager;
        civManager.CMDAddOwnedObject(building.gameObject);        
        building.SetSide(this.Side,building.GetComponent<OutlineObj>());
        civManager.CMDSetTeamColor(building.gameObject);
        Result.HideRange(this,Movement); 
        civManager.CMDShowAllUnits();
        UnitManager.Instance.selectedUnit = null;
        NetworkServer.Destroy(this.gameObject);
        civManager.CMDHideAllUnits();
    }
    #endregion

    #region  setside
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
    public void SetSide(Side side, OutlineObj outline)
    {
        this.Side = side;
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
    public void StartCaptureCoroutine(NetworkIdentity identity,GameObject sideable,CivManager civManager)
    {
        StartCoroutine(CaptureCoroutine(civManager));
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
}

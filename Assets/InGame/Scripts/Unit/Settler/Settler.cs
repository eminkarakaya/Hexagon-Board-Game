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
    [SerializeField] protected GameObject buildingPrefab;
    public Hex Hex { get => _hex; set{_hex = value;} }
    [SerializeField] private Hex _hex;
    public Movement Movement { get; set; }
    public MovementSystem Result { get; set; }
    public Vector3Int Position { get; set; }
    [SerializeField] Canvas canvas;
    public Canvas Canvas { get => canvas; set{canvas = value;} }
    public Outline Outline { get; set; }
    public Side Side { get =>_side; set{_side = value;} }
    [SerializeField] private List<GameObject> visions;
    public List<GameObject> Visions=>visions;

    public Vision Vision { get; set; }
    public IMovable Movable { get; set; }

    [SerializeField] private Side _side;
    #endregion

    #region Mirror and Unity callbacks
    public override void OnStopAuthority()
    {
        // CloseCanvas();
        // civManager.CMDHideAllUnits();
        // Movement.HideRangeStopAuthority();
        UnitManager.Instance.ClearOldSelection();
    }
    

    private void Awake() {

    }
    private void Start() {
        Outline = GetComponent<Outline>();
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
    public virtual void DeselectSettler()
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
        // Result.ShowRange(this,Movement);
        Result.ShowRange(this,Movement);
        SelectSettler();
    }

    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);

    }

    public void RightClick(Hex selectedHex)
    {
        HexGrid hexGrid =FindObjectOfType<HexGrid>();
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid,1);
        if(TryGetComponent(out SettlerMovementSeaAndLand settlerMovementSeaAndLand))
        {
            Result = new SettlerMovementSystemSeaAndLand(this);
        }
        else
            Result = new SettlerMovementSystem(this);
        Result.CalculateRange(this,hexGrid);
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid,1);
        Result.MoveUnit(Movement,FindObjectOfType<HexGrid>(),selectedHex);
    }

    public void RightClick2(Hex selectedHex)
    {
    }
    #endregion
    
    #region  createBuilding

    public void CreateBuildingOnClick()
    {
        CMDCreateBuilding();
        TaskComplate();
    }
    [Command]
    public virtual void CMDCreateBuilding()
    {
        if(Hex.Building != null) return;
        Building unit = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateBuilding(unit);
    }
    [ClientRpc] // server -> client
    protected void RPCCreateBuilding(Building building)
    {
        civManager.CMDRemoveOwnedObject(this.gameObject);
        building.transform.position = new Vector3 (Hex.transform.position.x , 1 , Hex.transform.position.z );
        building.transform.rotation = Quaternion.Euler(-90,0,0);
        building.Hex = Hex;
        building.Hex.Building = building;
        building.CivManager = civManager;
        civManager.CMDAddOwnedObject(building.gameObject);
        var buildings = FindObjectsOfType<Building>().ToList();
        foreach (var item in buildings)
        {
            if(item == null) continue;
            if(item.isOwned)
            {
                item.SetSide(Side.Me,item.GetComponent<Outline>());
            }
            else
                item.SetSide(Side.Enemy,item.GetComponent<Outline>());
        }
        civManager.SetTeamColor(building.gameObject);
        Result.HideRange(this,Movement);  
        UnitManager.Instance.selectedUnit = null;
        civManager.DestroyObj(this.gameObject);
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
    
    public IEnumerator wait(CivManager attackableCivManager)
    {
        this.civManager.CMDRemoveOwnedObject(this.gameObject); //requestauthority = false
        this.CivManager = attackableCivManager;
        while(GetComponent<NetworkIdentity>().isOwned == false)
        {
            yield return null;

        }
        CMDSetSide(attackableCivManager);
        attackableCivManager.SetTeamColor(this.gameObject);
        
        attackableCivManager.CMDShowAllUnits();
        attackableCivManager.CMDAddOwnedObject(this.gameObject); //requestauthority = false

    }
    public void SetSide(Side side, Outline outline)
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
        else
            outline.OutlineColor = Color.blue;
    }
    public void StartCoroutine1(NetworkIdentity identity,GameObject sideable,CivManager civManager)
    {
        // civManager.Capture(identity);
        StartCoroutine(wait(civManager));
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
        Debug.Log("TASK COMPLATE");
    }

    public void TaskReset()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}

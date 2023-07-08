using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
public class Settler : NetworkBehaviour , IMovable , ISelectable ,IVisionable ,ISideable
{
    #region  Props
        [SyncVar] [SerializeField] private  CivManager civManager;
    public CivManager CivManager {get => civManager;set {civManager = value;}}

    [SerializeField] private GameObject buildingPrefab;
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
        Result = new SettlerMovableResult(this);
    }
    #endregion
    
    #region  selectable methods
    public void CloseCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }

    public void Deselect()
    {
        Result.HideRange(this,Movement);
    }


    public void LeftClick()
    {
        Outline.enabled = true;
        // Result.ShowRange(this,Movement);
        Result.ShowRange(this,Movement);
    }

    public void OpenCanvas()
    {
        Canvas.gameObject.SetActive(true);

    }

    public void RightClick(Hex selectedHex)
    {
        HexGrid hexGrid =FindObjectOfType<HexGrid>();
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid,1);
        Result = new SettlerMovableResult(this);
        Result.CalculateRange(this,hexGrid);
        Result.ShowPath(selectedHex.HexCoordinates,hexGrid,1);
    }

    public void RightClick2(Hex selectedHex)
    {
        Result.MoveUnit(Movement,FindObjectOfType<HexGrid>(),selectedHex);
    }
    #endregion
    
    #region  createBuilding
    [Command]
    public void CreateBuilding_OnClick()
    {
        if(Hex.Building != null) return;
        Building unit = Instantiate(buildingPrefab).GetComponent<Building>();
        NetworkServer.Spawn(unit.gameObject,connectionToClient);
        RPCCreateBuilding(unit);
        
    }
    [ClientRpc] // server -> client
    private void RPCCreateBuilding(Building building)
    {

        building.transform.position = new Vector3 (Hex.transform.position.x , 1 , Hex.transform.position.z );
        building.transform.rotation = Quaternion.Euler(-90,0,0);
        building.Hex = Hex;
        building.Hex.Building = building;
        building.CivManager = civManager;
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
        civManager.SetTeamColor(this.gameObject);
        
        civManager.CMDShowAllUnits();
        
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
        StartCoroutine(wait(identity,sideable,civManager));
    }
    #endregion
}

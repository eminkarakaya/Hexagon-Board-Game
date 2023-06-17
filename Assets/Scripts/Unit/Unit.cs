using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
[SelectionBase]
public class Unit : NetworkBehaviour
{
    Outline outline;
    public HP hp;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private HexGrid hexGrid;
    [SyncVar] [SerializeField] private Hex hex;
    public Hex Hex {get => hex; set {hex = value;}}
    [SerializeField] private GameObject canvas;
    [SerializeField] private Side side;
    public Side Side {get => side;}
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
    [SerializeField] private int sightDistance = 2;
    public int SightDistance {get => sightDistance;}
    [SerializeField] private int _movementPoints = 20;
    public int MovementPoints {get => _movementPoints;}
    [SerializeField] private int _currentMovementPoints = 20;
    [SerializeField] private float movementDuration = 1, rotationDuration = .3f;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    private Queue<Hex> pathHexes = new Queue<Hex>();
    public event System.Action<Unit> MovementFinished;    
    
    [SerializeField] private List<GameObject> sight;
    public List<GameObject> Sight{get => sight;}
    SightResult sightRange;
    private void Start() {
        hp = GetComponent<HP>();
        
    }
    public void OpenCanvas()
    {
        canvas.SetActive(true);
    }
    public void CloseCanvas()
    {
        canvas.SetActive(false);

    }
    public void ShowSight(Hex hex)
    {
        if(Side == Side.Enemy) return;
        // HideSight(hex);
        sightRange = GraphSearch.GetRangeSightDistance(hex.HexCoordinates,sightDistance,hexGrid);
        foreach (var item in sightRange.GetRangeSight())
        {
            if(hexGrid.GetTileAt(item) != null)
            {
                Hex hex1 = hexGrid.GetTileAt(item);
                hex1.OpenLinkedObjectSight();
                hex1.isVisible=true;
            }
        }
    }

    public void HideSight(Hex hex)
    {
        hexGrid = FindObjectOfType<HexGrid>();
        if(sightRange.sightNodesDict == null) 
        {
            sightRange = GraphSearch.GetRangeSightDistance(hex.HexCoordinates,sightDistance,hexGrid);
        }
        // if(Side == Side.Enemy) return;
        foreach (var item in sightRange.sightNodesDict)
        {
            Hex [] hexes = FindObjectsOfType<Hex>();
           
            Hex hex1 = hexGrid.GetTileAt(item.Key);
            hex1.CloseLinkedObjectSight();
            hex.isVisible = false;
        }
    }
   
    public override void OnStartAuthority()
    {
        outline = GetComponent<Outline>();
    }
    public override void OnStartClient()
    {
    }
   
    
    public int GetCurrentMovementPoints()
    {
        return _currentMovementPoints;
    }
    public void SetCurrentMovementPoints(int value)
    {
        _currentMovementPoints = value;
    }
    internal void Deselect()
    {
        outline.enabled = false;
        
    }
    internal void Select()
    {
        outline.enabled = true;
    }

    internal void MoveThroughPath(List<Vector3> currentPathTemp,List<Hex> currentPath ,Hex lastHex ,bool isMove = true)
    {
        pathPositions = new Queue<Vector3>(currentPathTemp);
        pathHexes = new Queue<Hex>(currentPath);
        
        if(currentPathTemp.Count == 0) return;
        Vector3 firstTarget = pathPositions.Dequeue();
        Hex firstHex = pathHexes.Dequeue();
        StartCoroutine(RotationCoroutine(firstTarget,firstHex,lastHex,rotationDuration,isMove));
    }
    [ClientRpc] private void RPCSetHex(Hex hex,Hex prevHex)
    {
        prevHex.Unit = null;
        this.Hex = hex;
        hex.Unit = this;
    }
     [ClientRpc] private void RPCChangeHex(Hex hex,Hex prevHex)
    {
        Hex tempHex = hex;
        hex = prevHex;
        prevHex = tempHex;
    }
    private IEnumerator RotationCoroutine(Vector3 endPos,Hex endHex,Hex hex, float rotationDuration,bool isMove = true)
    {
        Quaternion startRotation = transform.rotation;
        endPos.y = transform.position.y;
        Vector3 direction = endPos - transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction,Vector3.up);

        if(Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation,endRotation)),1f) == false)
        {
            float timeElapsed = 0;
            while(timeElapsed < rotationDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / rotationDuration;
                transform.rotation = Quaternion.Lerp(startRotation,endRotation,lerpStep);
                yield return null;
            }
            transform.rotation = endRotation;
        }
        if(isMove)
        {
            StartCoroutine(MovementCoroutine(endPos,endHex,hex));
        }
        else
        {
            startRotation = transform.rotation;
            direction = new Vector3(hex.transform.position.x,transform.position.y,hex.transform.position.z) - transform.position;
            endRotation = Quaternion.LookRotation(direction,Vector3.up);
            float timeElapsed = 0;
            while(timeElapsed < rotationDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / rotationDuration;
                transform.rotation = Quaternion.Lerp(startRotation,endRotation,lerpStep);
                yield return null;
            }
            transform.rotation = endRotation;

            if(TryGetComponent(out Attack attack))
                {
                    if(hex.Building != null)
                        attack.AttackUnit(hex.Building.hp);
                    else
                        attack.AttackUnit(hex.Unit.hp);

                }
            MovementFinished?.Invoke(this);
        }

    }
    
    [Command] private void CMDSetHex(Hex hex,Hex prevHex)
    {
        RPCSetHex(hex,prevHex);
    }
    [Command] private void CMDChangeHex(Hex hex1,Hex hex2)
    {
        RPCChangeHex(hex1,hex2);
    }
    private IEnumerator MovementCoroutine(Vector3 endPos,Hex endHex,Hex hex)
    {
        Vector3 startPos = transform.position;
        endPos.y = startPos.y;
        if(endHex.IsEnemy()) 
        {
            yield break;
        }

        // Movementstart
        float timeElapsed = 0f;
        while(timeElapsed<movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration;
            transform.position = Vector3.Lerp(startPos,endPos,lerpStep);
            yield return null;
        }
        // MovementFinish





        // MovementFinishEvents
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.CMDHideAllUnits();
        CMDHide();
        
        CMDSetHex(endHex,Hex);
        this.Hex = endHex;
        
        transform.position = endPos;
        playerManager.CMDShowAllUnits();
        CMDShow();
        
        if(pathPositions.Count > 0)
        {
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(),pathHexes.Dequeue(),hex,rotationDuration));
        }
        else
        {
            
            MovementFinished?.Invoke(this);
            if(hex.Unit != null && hex.Unit.Side == Side.Enemy)
            {
                Quaternion startRotation = transform.rotation;
                Vector3 direction = new Vector3(hex.transform.position.x,transform.position.y,hex.transform.position.z) - transform.position;
                Quaternion endRotation = Quaternion.LookRotation(direction,Vector3.up);
                timeElapsed = 0;
                while(timeElapsed < rotationDuration)
                {
                    timeElapsed += Time.deltaTime;
                    float lerpStep = timeElapsed / rotationDuration;
                    transform.rotation = Quaternion.Lerp(startRotation,endRotation,lerpStep);
                    yield return null;
                }
                transform.rotation = endRotation;
                if(TryGetComponent(out Attack attack))
                {
                    if(hex.Building != null)
                        attack.AttackUnit(hex.Building.hp);
                    else
                        attack.AttackUnit(hex.Unit.hp);

                }
            }
        }
    }
    [Command]
    private void CMDChangeHexes(Hex hex1, Hex hex2)
    {
        ChangeHexes(hex1,hex2);
    }

    [ClientRpc]
    private void ChangeHexes(Hex hex1, Hex hex2)
    {
        Unit tempUnit = hex1.Unit;
        hex1.Unit = hex2.Unit;
        hex2.Unit = tempUnit;


        hex1.Unit.hex = hex1;
        hex2.Unit.hex = hex2;   
    }
    public void ChangeHex(Unit firstUnit,Unit targetUnit)
    {
        // BFSResult result = GraphSearch.GetRange(hexGrid,targetUnit.hex.HexCoordinates,targetUnit.MovementPoints);

        if(MovementSystem.Instance.IsHexInRange(firstUnit.hex.HexCoordinates) && MovementSystem.Instance.IsHexInRange(targetUnit.hex.HexCoordinates,firstUnit.hex.HexCoordinates,targetUnit.GetCurrentMovementPoints()))
        {

        Vector3 startPos = firstUnit.hex.transform.position;
        startPos.y = 1;
        Vector3 endPos = targetUnit.hex.transform.position;
        endPos.y = 1;
        StartCoroutine(RotationUnit(firstUnit,targetUnit,endPos,startPos,rotationDuration));
        StartCoroutine(RotationUnit(targetUnit,firstUnit,startPos,endPos,rotationDuration));
        // StartCoroutine(MoveUnit(firstUnit,endPos,startPos,movementDuration));
        // StartCoroutine(MoveUnit(targetUnit,startPos,endPos,movementDuration));
        
        CMDChangeHexes(firstUnit.hex,targetUnit.hex);
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.CMDHideAllUnits();
        CMDHide();
        
        // CMDChangeHex(firstUnit.hex,targetUnit.hex);
        
        playerManager.CMDShowAllUnits();
        CMDShow();
        }   
        
    }
    private IEnumerator RotationUnit(Unit firstUnit,Unit targetUnit,Vector3 endPos,Vector3 startPos, float rotationDuration)
    {
        Quaternion startRotation = firstUnit.transform.rotation;
        endPos.y = firstUnit.transform.position.y;
        Vector3 direction = endPos - firstUnit.transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction,Vector3.up);

        // Quaternion startRotation = firstUnit.transform.rotation;
        // Vector3 endPos = firstUnit.transform.position;
        // endPos.y = targetUnit.transform.position.y;
        // Vector3 direction = endPos - firstUnit.transform.position;
        // Quaternion endRotation = Quaternion.LookRotation(direction,Vector3.up);

        // if(Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation,endRotation)),1f) == false)
        // {
            float timeElapsed = 0;
            while(timeElapsed < rotationDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / rotationDuration;
                firstUnit.transform.rotation = Quaternion.Lerp(startRotation,endRotation,lerpStep);
                yield return null;
            }
            
            StartCoroutine(MoveUnit(firstUnit,endPos,startPos,movementDuration));
            
        // }
    }
    private IEnumerator MoveUnit(Unit unit,Vector3 endPos,Vector3 startPos,float movementDuration )
    {
        //  startPos = unit.transform.position;
        float timeElapsed = 0f;
        while(timeElapsed<movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration;
            unit.transform.position = Vector3.Lerp(startPos,endPos,lerpStep);
            yield return null;
        }
            
    }
    [Command]
    private void CMDShow()
    {
        RPCShow();
    }
    [ClientRpc]
    private void RPCShow()
    {
        MovementSystem.Instance.RPCShowRange(UnitManager.Instance.selectedUnit,this);

    }
    [ClientRpc]
    private void RPCHide()
    {
        MovementSystem.Instance.RPCHideRange(this);   
        
    }
    [Command]
    private void CMDHide()
    {  
        RPCHide();
    }
}
public enum Side
{
    Ally,
    Me,
    Enemy
}
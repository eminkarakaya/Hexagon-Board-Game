using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Movement : NetworkBehaviour
{
    protected IMovable Moveable;
    [SerializeField] protected CivManager playerManager;
    [SerializeField] protected float movementDuration = 1, rotationDuration = .3f;
    public event System.Action<Movement> MovementFinished;
    protected Queue<Hex> pathHexes = new Queue<Hex>();
    [SerializeField] protected int _movementPoints = 20;
    public int MovementPoints {get => _movementPoints;}
    [SerializeField] protected int _currentMovementPoints = 20;
    protected Queue<Vector3> pathPositions = new Queue<Vector3>();
    
    protected void Start() {
        if(playerManager == null)
            playerManager = PlayerManager.FindPlayerManager();
        Moveable = GetComponent<IMovable>();
    }
     public void SetCurrentMovementPoints(int value)
    {
        _currentMovementPoints = value;
    }

     public int GetCurrentMovementPoints()
    {
        return _currentMovementPoints;
    }
    [ClientRpc] protected void RPCSetHex(Hex hex,Hex prevHex) // parametreler hex degıl IMoveable olcak
    {
        
        if(TryGetComponent(out Unit unit))
        {
            prevHex.Unit = null;
            this.Moveable.Hex = hex;
            hex.Unit = unit;
        }
        else if(TryGetComponent(out Settler settler))
        {
            prevHex.Settler = null;
            this.Moveable.Hex = hex;
            hex.Settler = settler;            
        }

    }
     [ClientRpc] protected void RPCChangeHex(Hex hex,Hex prevHex)
    {
        Hex tempHex = hex;
        hex = prevHex;
        prevHex = tempHex;
    }
    [Command] protected void CMDSetHex(Hex hex,Hex prevHex)
    {
        RPCSetHex(hex,prevHex);
    }
    protected void MovementFinsihEvent(Movement movement)
    {
        MovementFinished?.Invoke(movement);
    }

    internal void MoveThroughPath(List<Vector3> currentPathTemp,List<Hex> currentPath ,Hex lastHex,MovementSystem movementSystem ,bool isMove = true)
    {
        pathPositions = new Queue<Vector3>(currentPathTemp);
        pathHexes = new Queue<Hex>(currentPath);

        if(currentPathTemp.Count == 0) return;
        Vector3 firstTarget = pathPositions.Dequeue();
        Hex firstHex = pathHexes.Dequeue();
        StartCoroutine(RotationCoroutine(firstTarget,firstHex,lastHex,rotationDuration,movementSystem,isMove));
    }
    protected IEnumerator RotationCoroutine(Vector3 endPos,Hex endHex,Hex hex, float rotationDuration,MovementSystem movementSystem,bool isMove = true)
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
            StartCoroutine(MovementCoroutine(endPos,endHex,hex,movementSystem));
        }
        else
        {
            if(hex.IsEnemy() ||hex.IsEnemyBuilding())
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
                AttackUnit(hex);
                MovementFinished?.Invoke(this);
            }
        }

    }
    public void AttackUnit(Hex hex)
    {
        if(TryGetComponent(out Attack attack))
        {
            if(hex.Building != null)
            {

                attack.AttackUnit(hex.Building);
            }
            else
            {
            attack.AttackUnit(hex.Unit);

            }

        }
    }
    public void StartCoroutineRotationUnit(Movement firstUnit,Vector3 endPos,Hex hex)
    {
        StartCoroutine(RotationUnit(firstUnit,endPos,hex));
    }
    public IEnumerator RotationUnit(Movement firstUnit,Vector3 endPos,Hex hex)
    {
        Quaternion startRotation = firstUnit.transform.rotation;
        endPos.y = firstUnit.transform.position.y;
        Vector3 direction = endPos - firstUnit.transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction,Vector3.up);

        
            float timeElapsed = 0;
            while(timeElapsed < rotationDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / rotationDuration;
                firstUnit.transform.rotation = Quaternion.Lerp(startRotation,endRotation,lerpStep);
                yield return null;
            }
            
        firstUnit.AttackUnit(hex);
        MovementFinished?.Invoke(this);
        // }
    }
    


    protected abstract IEnumerator MovementCoroutine(Vector3 endPos,Hex endHex,Hex hex,MovementSystem movementSystem);
    [Command]
    protected void CMDShow()
    {
        RPCShow();
    }
    [ClientRpc]
    protected void RPCShow()
    {
        MovementSystem movementSystem = new UnitMovableResult(Moveable);
        // movementSystem.CalculateRange(Moveable,FindObjectOfType<HexGrid>());
        if(UnitManager.Instance.selectedUnit != null && UnitManager.Instance.selectedUnit.Movable != null)
        {
            // Debug.Log("true");
            // Debug.Log(UnitManager.Instance.selectedUnit + " UnitManager.Instance.selectedUnit");
            // if(UnitManager.Instance.selectedUnit != null)
                // Debug.Log(UnitManager.Instance.selectedUnit.Movable + " UnitManager.Instance.selectedUnit.moveable");
            movementSystem.ShowRange(UnitManager.Instance.selectedUnit.Movable,UnitManager.Instance.selectedUnit.Movable.Movement);
        }
        else{
            // Debug.Log(UnitManager.Instance.selectedUnit + " UnitManager.Instance.selectedUnit");
            // if(UnitManager.Instance.selectedUnit != null)
                // Debug.Log(UnitManager.Instance.selectedUnit.Movable + " UnitManager.Instance.selectedUnit.moveable");
        }
    }
    [ClientRpc]
    protected void RPCHide()
    {
        MovementSystem movementSystem = new UnitMovableResult(Moveable);
        // movementSystem.CalculateRange(Moveable,FindObjectOfType<HexGrid>());
        // // Debug.Log(UnitManager.Instance);
        // // Debug.Log(UnitManager.Instance.selectedUnit);
        // // Debug.Log(UnitManager.Instance.selectedUnit.Movable +  " UnitManager.Instance.selectedUnit.Movable ");
        if(UnitManager.Instance.selectedUnit != null && UnitManager.Instance.selectedUnit.Movable != null)
        {
            // Debug.Log("true");
            movementSystem.HideRange(UnitManager.Instance.selectedUnit.Movable,UnitManager.Instance.selectedUnit.Movable.Movement);
            // Debug.Log(UnitManager.Instance.selectedUnit + " UnitManager.Instance.selectedUnit");
            // if(UnitManager.Instance.selectedUnit != null)
                // Debug.Log(UnitManager.Instance.selectedUnit.Movable + " UnitManager.Instance.selectedUnit.moveable");
        }
        else
        {
            // Debug.Log(UnitManager.Instance.selectedUnit + " UnitManager.Instance.selectedUnit");
            // if(UnitManager.Instance.selectedUnit != null)
                // Debug.Log(UnitManager.Instance.selectedUnit.Movable + " UnitManager.Instance.selectedUnit.moveable");
        }
    }
    [Command]
    protected void CMDHide(Movement movement)
    {
       RPCHide();
    }
    [Command]
    protected void CMDChangeHexes(Hex hex1, Hex hex2)
    {
        ChangeHexes(hex1,hex2);
    }

    [ClientRpc]
    protected void ChangeHexes(Hex hex1, Hex hex2)
    {
        if(TryGetComponent(out Unit unit))
        {
            Unit tempUnit = hex1.Unit;
            hex1.Unit = hex2.Unit;
            hex2.Unit = tempUnit;


            hex1.Unit.Hex = hex1;
            hex2.Unit.Hex = hex2;
        }
        else if(TryGetComponent(out Settler settler))
        {
            Settler tempUnit = hex1.Settler;
            hex1.Settler = hex2.Settler;
            hex2.Settler = tempUnit;


            hex1.Settler.Hex = hex1;
            hex2.Settler.Hex = hex2;

        }
    }

    
    public void ChangeHex(Movement firstUnit,Movement targetUnit,MovementSystem movementSystem)
    {
        // BFSResult result = GraphSearch.GetRange(hexGrid,targetUnit.hex.HexCoordinates,targetUnit.MovementPoints);

        if(movementSystem.IsHexInRange(firstUnit.Moveable.Hex.HexCoordinates) && movementSystem.IsHexInRange(targetUnit.Moveable.Hex.HexCoordinates,firstUnit.Moveable.Hex.HexCoordinates,targetUnit.GetCurrentMovementPoints()))
        {

            Vector3 startPos = firstUnit.Moveable.Hex.transform.position;
            startPos.y = 1;
            Vector3 endPos = targetUnit.Moveable.Hex.transform.position;
            endPos.y = 1;
            StartCoroutine(RotationUnit(firstUnit,targetUnit,endPos,startPos,rotationDuration));
            StartCoroutine(RotationUnit(targetUnit,firstUnit,startPos,endPos,rotationDuration));
            // StartCoroutine(MoveUnit(firstUnit,endPos,startPos,movementDuration));
            // StartCoroutine(MoveUnit(targetUnit,startPos,endPos,movementDuration));

            CMDChangeHexes(firstUnit.Moveable.Hex,targetUnit.Moveable.Hex);
            playerManager = FindObjectOfType<PlayerManager>();
            playerManager.CMDHideAllUnits();


            // CMDChangeHex(firstUnit.hex,targetUnit.hex);

            playerManager.CMDShowAllUnits();

        }

    }
    protected IEnumerator RotationUnit(Movement firstUnit,Movement targetUnit,Vector3 endPos,Vector3 startPos, float rotationDuration)
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

            StartCoroutine(MoveUnit(firstUnit,endPos,startPos));

        // }
    }
    protected IEnumerator MoveUnit(Movement unit,Vector3 endPos,Vector3 startPos )
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
}

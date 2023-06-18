using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Movement : NetworkBehaviour
{
    Unit Unit;
    Settler settler;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private float movementDuration = 1, rotationDuration = .3f;
    public event System.Action<Movement> MovementFinished;
    private Queue<Hex> pathHexes = new Queue<Hex>();
    [SerializeField] private int _movementPoints = 20;
    public int MovementPoints {get => _movementPoints;}
    [SerializeField] private int _currentMovementPoints = 20;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    private void Start() {
        Unit = GetComponent<Unit>();
    }
     public void SetCurrentMovementPoints(int value)
    {
        _currentMovementPoints = value;
    }

     public int GetCurrentMovementPoints()
    {
        return _currentMovementPoints;
    }
    [ClientRpc] private void RPCSetHex(Hex hex,Hex prevHex)
    {
        prevHex.Unit = null;
        this.Unit.Hex = hex;
        hex.Unit = this.GetComponent<Unit>();
    }
     [ClientRpc] private void RPCChangeHex(Hex hex,Hex prevHex)
    {
        Hex tempHex = hex;
        hex = prevHex;
        prevHex = tempHex;
    }
    [Command] private void CMDSetHex(Hex hex,Hex prevHex)
    {
        RPCSetHex(hex,prevHex);
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
    private IEnumerator RotationCoroutine(Vector3 endPos,Hex endHex,Hex hex, float rotationDuration,MovementSystem movementSystem,bool isMove = true)
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
    private IEnumerator MovementCoroutine(Vector3 endPos,Hex endHex,Hex hex,MovementSystem movementSystem)
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
        
        CMDHide(this);
        CMDSetHex(endHex,Unit.Hex);
        this.Unit.Hex = endHex;

        transform.position = endPos;
        playerManager.CMDShowAllUnits();
        CMDShow();
        
        if(pathPositions.Count > 0)
        {
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(),pathHexes.Dequeue(),hex,rotationDuration,movementSystem));
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
    private void CMDShow()
    {
        RPCShow();
    }
    [ClientRpc]
    private void RPCShow()
    {
        
        MovementSystem movementSystem = new UnitMovableResult(this);
        movementSystem.ShowRange(UnitManager.Instance.selectedUnit,this);

        
        

    }
    [ClientRpc]
    private void RPCHide()
    {
       

        MovementSystem movementSystem = new UnitMovableResult(this);
        movementSystem.HideRange(this);

        
    }
    [Command]
    private void CMDHide(Movement movement)
    {
       RPCHide();
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


        hex1.Unit.Hex = hex1;
        hex2.Unit.Hex = hex2;
    }
    public void ChangeHex(Movement firstUnit,Movement targetUnit,MovementSystem movementSystem)
    {
        // BFSResult result = GraphSearch.GetRange(hexGrid,targetUnit.hex.HexCoordinates,targetUnit.MovementPoints);

        if(movementSystem.IsHexInRange(firstUnit.Unit.Hex.HexCoordinates) && movementSystem.IsHexInRange(targetUnit.Unit.Hex.HexCoordinates,firstUnit.Unit.Hex.HexCoordinates,targetUnit.GetCurrentMovementPoints()))
        {

            Vector3 startPos = firstUnit.Unit.Hex.transform.position;
            startPos.y = 1;
            Vector3 endPos = targetUnit.Unit.Hex.transform.position;
            endPos.y = 1;
            StartCoroutine(RotationUnit(firstUnit,targetUnit,endPos,startPos,rotationDuration));
            StartCoroutine(RotationUnit(targetUnit,firstUnit,startPos,endPos,rotationDuration));
            // StartCoroutine(MoveUnit(firstUnit,endPos,startPos,movementDuration));
            // StartCoroutine(MoveUnit(targetUnit,startPos,endPos,movementDuration));

            CMDChangeHexes(firstUnit.Unit.Hex,targetUnit.Unit.Hex);
            playerManager = FindObjectOfType<PlayerManager>();
            playerManager.CMDHideAllUnits();


            // CMDChangeHex(firstUnit.hex,targetUnit.hex);

            playerManager.CMDShowAllUnits();

        }

    }
    private IEnumerator RotationUnit(Movement firstUnit,Movement targetUnit,Vector3 endPos,Vector3 startPos, float rotationDuration)
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
    private IEnumerator MoveUnit(Movement unit,Vector3 endPos,Vector3 startPos,float movementDuration )
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

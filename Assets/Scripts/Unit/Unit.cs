using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    [SerializeField] private Hex hex;
    public Hex Hex {get => hex; set {hex = value;}}
    [SerializeField] private Side side;
    public Side Side {get => side;}

    [SerializeField] private int _movementPoints = 20;
    public int MovementPoints {get => _movementPoints;}
    [SerializeField] private int _currentMovementPoints = 20;
    [SerializeField] private float movementDuration = 1, rotationDuration = .3f;
    private GlowHighlight glowHighlight;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();

    public event System.Action<Unit> MovementFinished;    
    public event System.Action<Unit> Selected;
    public event System.Action<Unit> Deselected;

    private void Awake() {
        glowHighlight = GetComponent<GlowHighlight>();
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
        glowHighlight.ToggleGlow(false);
        
    }
    internal void Select()
    {
        glowHighlight.ToggleGlow();
    }
    internal void MoveThroughPath(List<Vector3> currentPath ,Hex lastHex ,bool isMove = true)
    {
        pathPositions = new Queue<Vector3>(currentPath);
        
        if(currentPath.Count == 0) return;
        Vector3 firstTarget = pathPositions.Dequeue();
        StartCoroutine(RotationCoroutine(firstTarget,lastHex,rotationDuration,isMove));
    }
    private IEnumerator RotationCoroutine(Vector3 endPos,Hex hex, float rotationDuration,bool isMove = true)
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
            StartCoroutine(MovementCoroutine(endPos,hex));
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
                attack.AttackUnit(hex.Unit);
            }
            MovementFinished?.Invoke(this);
        }
        // if(pathPositions.Count == 0)
        // {
        //    if(TryGetComponent(out Melee melee))
        //             melee.AttackUnit(hex.Unit);
        // }
        
    }
    private IEnumerator MovementCoroutine(Vector3 endPos,Hex hex)
    {
        Vector3 startPos = transform.position;
        endPos.y = startPos.y;
        float timeElapsed = 0f;
        while(timeElapsed<movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration;
            transform.position = Vector3.Lerp(startPos,endPos,lerpStep);
            yield return null;
        }
        transform.position = endPos;

        if(pathPositions.Count > 0)
        {
            // Debug.Log("Selecting the next position");
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(),hex,rotationDuration));
            Debug.Log(pathPositions.Count + " pathpositioncount");
        }
        else
        {
            Debug.Log(hex.HexCoordinates);
            MovementFinished?.Invoke(this);
            if(hex.Unit != null && hex.Unit.side == Side.Enemy)
            {
                Debug.Log(pathPositions.Count + " pathpositioncount   1");
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
                    attack.AttackUnit(hex.Unit);
            }
        }
    }
}
public enum Side
{
    Ally,
    Enemy
}
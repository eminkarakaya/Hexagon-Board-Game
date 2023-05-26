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
    [SerializeField] private int sightDistance = 2;
    public int SightDistance {get => sightDistance;}
    [SerializeField] private int _movementPoints = 20;
    public int MovementPoints {get => _movementPoints;}
    [SerializeField] private int _currentMovementPoints = 20;
    [SerializeField] private float movementDuration = 1, rotationDuration = .3f;
    private GlowHighlight glowHighlight;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    private Queue<Hex> pathHexes = new Queue<Hex>();
    public event System.Action<Hex> HexChanges;
    public event System.Action<Unit> MovementFinished;    
    
    [SerializeField] private List<GameObject> sight;
    public List<GameObject> Sight{get => sight;}
    SightResult sightRange;
    
    public void ShowSight(Hex hex)
    {
        if(side == Side.Enemy) return;
        HideSight(hex);
        sightRange = GraphSearch.GetRangeSightDistance(hex.HexCoordinates,sightDistance);
        HexGrid hexGrid = HexGrid.Instance;
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
        if(sightRange.sightNodesDict == null) return;
        if(side == Side.Enemy) return;
        HexGrid hexGrid = HexGrid.Instance;
        foreach (var item in sightRange.sightNodesDict)
        {
            Hex hex1 = hexGrid.GetTileAt(item.Key);
            hex1.CloseLinkedObjectSight();
            hex.isVisible = false;
        }
    }
    private void Awake() {
        glowHighlight = GetComponent<GlowHighlight>();
    }
    private void OnEnable() {
        HexChanges += ShowSight;
    }
    private void OnDisable() {
        
        HexChanges -= ShowSight;
    }
    private void Start() {
        if(TryGetComponent(out Photon.Pun.PhotonView pw))
        {
            if(pw.IsMine)
                Select();
        }
        ShowSight(hex);
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
    internal void MoveThroughPath(List<Vector3> currentPathTemp,List<Hex> currentPath ,Hex lastHex ,bool isMove = true)
    {
        pathPositions = new Queue<Vector3>(currentPathTemp);
        pathHexes = new Queue<Hex>(currentPath);
        
        if(currentPathTemp.Count == 0) return;
        Vector3 firstTarget = pathPositions.Dequeue();
        Hex firstHex = pathHexes.Dequeue();
        StartCoroutine(RotationCoroutine(firstTarget,firstHex,lastHex,rotationDuration,isMove));
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
    private IEnumerator MovementCoroutine(Vector3 endPos,Hex endHex,Hex hex)
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

        Hex = endHex;
        transform.position = endPos;
        // HexChanges?.Invoke(endHex);
        GameManager.Instance.SightAllUnits();
        
        if(pathPositions.Count > 0)
        {
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(),pathHexes.Dequeue(),hex,rotationDuration));
        }
        else
        {
            MovementFinished?.Invoke(this);
            if(hex.Unit != null && hex.Unit.side == Side.Enemy)
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
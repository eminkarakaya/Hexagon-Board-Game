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
    [SerializeField] private float movementDuration = 1, rotationDuration = .3f;
    private GlowHighlight glowHighlight;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();

    public event System.Action<Unit> MovementFinished;

    private void Awake() {
        glowHighlight = GetComponent<GlowHighlight>();
    }
    internal void Deselect()
    {
        glowHighlight.ToggleGlow(false);
    }
    internal void Select()
    {
        glowHighlight.ToggleGlow();
    }
    internal void MoveThroughPath(List<Vector3> currentPath)
    {
        pathPositions = new Queue<Vector3>(currentPath);
        Vector3 firstTarget = pathPositions.Dequeue();
        StartCoroutine(RotationCoroutine(firstTarget,rotationDuration,true));

    }
    private IEnumerator RotationCoroutine(Vector3 endPos, float rotationDuration,bool firstRotation = true)
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
        StartCoroutine(MovementCoroutine(endPos));
    }
    private IEnumerator MovementCoroutine(Vector3 endPos)
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
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(),rotationDuration));

        }
        else
        {
            // Debug.Log("Movement Finished");
            MovementFinished?.Invoke(this);
        }
    }
}
public enum Side
{
    Ally,
    Enemy
}
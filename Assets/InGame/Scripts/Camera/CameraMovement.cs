using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private LayerMask mask;
    public LayerMask Mask{get => mask; set {
        mask = value;
        Camera.main.cullingMask = mask;
        }
    }
    
    [SerializeField] private Vector2 clampX,clampY;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _movementSpeed,_movementTime,_normalSpeed,_fastSpeed;
    [SerializeField] private Vector3 _newPosition,_zoomAmount,_newZoom;

    [SerializeField] private Vector3 _dragStartPos,_dragCurrentPos;
    private void Start() {
        // Debug.Log(transform.forward);
        // Debug.Log(Screen.height + " " + Screen.width);
        _newPosition = transform.position;
        _newZoom = _cameraTransform.localPosition;
    }
    private void Update() {
        HandleMovementInput();
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            _newZoom += Input.mouseScrollDelta.y * _zoomAmount;
        }
        if(Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up,Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    
            float entry;
            if(plane.Raycast(ray,out entry))
            {
                _dragStartPos = ray.GetPoint(entry);
            }
        }
        if(Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up,Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if(plane.Raycast(ray,out entry))
            {
                _dragCurrentPos = ray.GetPoint(entry);
                _newPosition = transform.position + _dragStartPos - _dragCurrentPos;
            }
        }
    }
    private void HandleMovementInput()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            _movementSpeed = _fastSpeed;
        }
        else
            _movementSpeed = _normalSpeed;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _newPosition += (transform.forward*_movementSpeed);
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {

            _newPosition += (transform.forward*-_movementSpeed);
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _newPosition += (transform.right*-_movementSpeed);

        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _newPosition += (transform.right*_movementSpeed);
            
        }
        if(Input.GetKey(KeyCode.R))
        {
            _newZoom += _zoomAmount;
        }
        if(Input.GetKey(KeyCode.F))
            _newZoom -= _zoomAmount;
        transform.position = Vector3.Lerp(transform.position,_newPosition,Time.deltaTime*_movementTime);
        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition,_newZoom,Time.deltaTime * _movementTime);
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x,clampX.x,clampX.y);
        pos.z = Mathf.Clamp(pos.z,clampY.x,clampY.y);
        transform.position = pos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent<Vector3> PointerClick;
    public UnityEvent<Vector3> PointerRightClick;
    private void Update() {
        DetectMouseClick();
        DetectMouseRightClick();
    }
    private void DetectMouseClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            PointerClick?.Invoke(mousePos);
        }
    }
    private void DetectMouseRightClick()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Input.mousePosition;
            PointerRightClick?.Invoke(mousePos);
        }
    }
}

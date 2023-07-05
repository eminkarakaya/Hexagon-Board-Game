using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent<Vector3> PointerClick;
    public UnityEvent<Vector3> PointerRightClick;
    public UnityEvent ESCEvent; 
    private void Update() {
        DetectMouseClick();
        DetectMouseRightClick();
        ESC();
    }
    private void ESC()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ESCEvent?.Invoke();
        }
        
    }
    private void DetectMouseClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector3 mousePos = Input.mousePosition;
            PointerClick?.Invoke(mousePos);
        }
    }
    private void DetectMouseRightClick()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector3 mousePos = Input.mousePosition;
            PointerRightClick?.Invoke(mousePos);
        }
    }
}

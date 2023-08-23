using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectWithUI : MonoBehaviour, IPointerClickHandler
{
    SelectionManager selectionManager;
    private void Start() {
        selectionManager = FindObjectOfType<SelectionManager>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Transform transform = eventData.pointerClick.transform;
        while(transform.parent != null)
        {
            transform = transform.parent;
            if(transform.TryGetComponent(out ISelectable selectable))
            {
                selectionManager.HandleClickUI(transform.gameObject);
                break;
            }
        }
    }
}
